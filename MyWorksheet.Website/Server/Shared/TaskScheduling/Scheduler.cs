using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services;
using MyWorksheet.Website.Server.Shared.Services.Logger;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using MyWorksheet.Website.Server.Shared.TaskScheduling.TaskRunners;
using MyWorksheet.Website.Shared.Services.Activation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Shared.TaskScheduling;

/// <summary>
///     Manages Tasks and the schedule that defines when they will run in the background
/// </summary>
[SingletonService(typeof(ISchedulerService))]
public class SchedulerService : RequireInit, ISchedulerService, IDisposable
{
    /// <summary>
    /// When this token is set it indicates a stop operation is in progress and the interal Task should be stopped
    /// </summary>
    private readonly CancellationTokenSource _innerControlToken;
    private readonly IAppLogger _logger;
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;

    /// <summary>
    /// If the event is set and we are currently in a pending wait operation, this operation will be stopped.
    /// </summary>
    private readonly AutoResetEvent _reinvalidateEvent;
    private readonly Thread _runner;
    private readonly IList<ITaskRunner> _taskRunners;

    public SchedulerService(IAppLogger logger, IDbContextFactory<MyworksheetContext> dbContextFactory) : this(CancellationToken.None, logger, dbContextFactory)
    {
    }

    /// <summary>
    ///     Instantiate a new Scheduler to run background tasks
    /// </summary>
    public SchedulerService(CancellationToken cancellationToken, IAppLogger logger, IDbContextFactory<MyworksheetContext> dbContextFactory)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _taskRunners = [];

        CancellationToken = cancellationToken;
        _innerControlToken = new CancellationTokenSource();
        _reinvalidateEvent = new AutoResetEvent(false);
        _runner = new Thread(RunScheduler);
    }

    public CancellationToken CancellationToken { get; }

    public bool IsRunning { get; private set; }

    /// <summary>
    ///     Close and clean up any opened resources
    /// </summary>
    public void Dispose()
    {
        // ensure the timer is stopped
        Stop();

        // clean up any tasks which are disposable
        foreach (var runner in _taskRunners)
        {
            if (runner.Task is IDisposable)
            {
                ((IDisposable)runner.Task).Dispose();
            }
        }
    }
    /// <summary>
    /// Gets the next interation.
    /// </summary>
    /// <value>
    /// The next interation.
    /// </value>
    public TimeSpan NextInteration
    {
        get
        {
            return _evaluatedTo - DateTime.UtcNow;
        }
    }

    private DateTime _evaluatedTo;

    /// <summary>
    /// Tries the add task.
    /// </summary>
    /// <param name="task">The task.</param>
    /// <param name="reinvalidate">The reinvalidate.</param>
    /// <returns></returns>
    public DynamicTaskRunner TryAddTask(ITask task, Func<DateTime> reinvalidate)
    {
        var defedRunner = new DynamicTaskRunner(task, reinvalidate, this);
        _taskRunners.Add(defedRunner);
        return defedRunner;
    }

    /// <summary>
    ///     Adds a task onto the schedule so it will be checked and run as defined by the schedule
    /// </summary>
    /// <param name="task">The task to run</param>
    /// <param name="schedule">The schedule that determines when the task should run</param>
    public bool TryAddTask(ITask task, Schedule schedule)
    {
        if (task == null)
        {
            throw new ArgumentNullException(nameof(task));
        }

        if (schedule == null)
        {
            throw new ArgumentNullException(nameof(schedule));
        }

        if (IsRunning)
        {
            return false;
        }

        switch (schedule.Type)
        {
            case ScheduleType.Periodical:
                var p = new PeriodicalTaskRunner(task, schedule.Frequency);
                _taskRunners.Add(p);
                break;

            case ScheduleType.Scheduled:
                var s = new ScheduledTaskRunner(task, schedule.RunAt);
                _taskRunners.Add(s);
                break;

            case ScheduleType.Task:
                var neverRunner = new OnDemandTaskRunner(task);
                _taskRunners.Add(neverRunner);
                break;
            case ScheduleType.Defered:

                break;
            default:
                throw new Exception(schedule.Type + " is not a supported schedule type.");
        }

        return true;
    }

    /// <summary>
    ///     Start checking if schedules are due to run their tasks
    /// </summary>
    public void Start()
    {
        lock (this)
        {
            if (IsRunning)
            {
                return;
            }
            _runner.Start();
        }
    }

    /// <summary>
    ///     Stop schedules checking if they are due to run
    /// </summary>
    public void Stop()
    {
        lock (this)
        {
            if (!IsRunning)
            {
                return;
            }
            _innerControlToken.Cancel();
        }
    }

    public event FailedTaskEventHandler OnFailedTask;

    /// <summary>
    /// Reinvalidates this instance. This will stop any pending wait operation and will ask all tasks for changed determinatedNextRun times.
    /// </summary>
    public void Reinvalidate()
    {
        _reinvalidateEvent.Set();
    }

    /// <summary>
    /// Reinvalidates this instance. This will stop any pending wait operation and will ask all tasks for changed determinatedNextRun times.
    /// </summary>
    public void Reinvalidate(DateTime future)
    {
        if (future < _evaluatedTo)
        {
            Reinvalidate();
        }
    }

    /// <summary>
    /// Returns a list of all Runners
    /// </summary>
    /// <returns></returns>
    public IReadOnlyCollection<ITaskRunner> Runners()
    {
        return _taskRunners.Where(e => e is not SingleTaskRunner).ToArray();
    }

    ~SchedulerService()
    {
        Dispose();
    }

    private void RunScheduler()
    {
        IsRunning = true;

        var waitAnyToken =
            CancellationTokenSource.CreateLinkedTokenSource(_innerControlToken.Token, CancellationToken);

        try
        {
            while (!waitAnyToken.IsCancellationRequested)
            {
                var maybeNextTaskRunner = _taskRunners.Select(f => f.DetermininateNextRun()).Min();
                var whenNext = maybeNextTaskRunner ?? TimeSpan.FromMinutes(1);

                _evaluatedTo = DateTime.UtcNow + whenNext;
                if (whenNext > TimeSpan.Zero)
                {
                    _logger.LogInformation($"Wait for external event or util '{_evaluatedTo}'", LoggerCategories.Scheduler.ToString());

                    var timeoutHandle = new CancellationTokenSource(whenNext).Token;
                    var waitHandles = new WaitHandle[]
                    {
                        _innerControlToken.Token.WaitHandle,
                        CancellationToken.WaitHandle,
                        _reinvalidateEvent,
                        timeoutHandle.WaitHandle
                    };
                    var whenAnyIndex = WaitHandle.WaitAny(waitHandles);
                    var expiredHandle = waitHandles[whenAnyIndex];

                    _logger.LogInformation($"Event or timeout reached", LoggerCategories.Scheduler.ToString());
                    if (expiredHandle == CancellationToken.WaitHandle || expiredHandle == _innerControlToken.Token.WaitHandle)
                    {
                        if (_innerControlToken.IsCancellationRequested || CancellationToken.IsCancellationRequested)
                        {
                            _logger.LogInformation($"Stop requested", LoggerCategories.Scheduler.ToString());
                            return;
                        }
                    }

                    if (expiredHandle == _reinvalidateEvent)
                    {
                        continue;
                    }
                }


                if (_taskRunners == null || _taskRunners.Count <= 0)
                {
                    _logger.LogInformation($"Event received but no runners present", LoggerCategories.Scheduler.ToString());
                    continue;
                }

                var tasks = new List<Task>();

                foreach (var runner in _taskRunners.ToArray())
                {
                    try
                    {
                        if (runner.Check())
                        {
                            _logger.LogInformation($"Run task {runner.Task.NamedTask}", LoggerCategories.Scheduler.ToString());
                            tasks.Add(RunTaskNow(runner, _logger));
                            if (runner is SingleTaskRunner)
                            {
                                _taskRunners.Remove(runner);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogInformation($"Run task {runner.Task.NamedTask} failed", LoggerCategories.Scheduler.ToString());
                        OnOnFailedTask(runner.Task, e, new ExceptionHandler());
                    }
                }

                var whenAll = Task.WhenAll(tasks);

                try
                {
                    whenAll.Wait(_innerControlToken.Token);
                }
                catch (Exception)
                {
                }

                // make sure any tasks are running, should be on their own threads
            }
        }
        finally
        {
            IsRunning = false;
        }
    }

    public void RunOnceNow(ITask task)
    {
        _taskRunners.Add(new SingleTaskRunner(task));
        Reinvalidate();
    }

    private Task RunTaskNow(ITaskRunner taskRunner, IAppLogger parentLogger)
    {
        var runKey = Guid.NewGuid().ToString("N");
        var logger = parentLogger.Copy();
        logger.Transform.Add(entry =>
        {
            var dbKey = $"SchedulerTask.{taskRunner.Task.NamedTask}.{runKey}";
            entry.OptionalData["DatabaseEntryKey"] = dbKey;
            return entry;
        });
        return taskRunner.Execute(logger);
    }

    protected virtual void OnOnFailedTask(ITask task, Exception exception, ExceptionHandler handler)
    {
        OnFailedTask?.Invoke(task, exception, handler);
    }

    public void AddByAttributes(Assembly inAssembly, ActivatorService activatorService)
    {
        foreach (var type in inAssembly.GetTypes())
        {
            var scheduleAt = type.GetCustomAttributes<ScheduleAttribute>(true).ToArray();
            if (!scheduleAt.Any())
            {
                continue;
            }

            var task = activatorService.ActivateType(type) as ITask;
            foreach (var scheduleAttribute in scheduleAt)
            {
                TryAddTask(task, scheduleAttribute.When());
            }
        }
    }

    public override void Init(IServiceProvider services)
    {
        base.Init(services);
        AddByAttributes(GetType().Assembly, services.GetRequiredService<ActivatorService>());
        Start();
    }
}