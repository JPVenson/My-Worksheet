using System;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;

namespace MyWorksheet.Website.Server.Shared.TaskScheduling.TaskRunners;

public class DynamicTaskRunner : ITaskRunner
{
    private readonly Func<DateTime> _reinvalidate;
    private readonly SchedulerService _attachedTo;

    public DynamicTaskRunner(ITask task, Func<DateTime> reinvalidate, SchedulerService attachedTo)
    {
        _reinvalidate = reinvalidate;
        _attachedTo = attachedTo;
        Task = task;
    }

    public void Reinvalidate()
    {
        RunAt = _reinvalidate();
        _attachedTo.Reinvalidate(RunAt.Value);
    }

    public DateTime? RunAt { get; set; }
    private DateTime _lastRun;

    public bool Check()
    {
        return RunAt <= DateTime.UtcNow;
    }

    public async Task Execute(IAppLogger logger)
    {
        _lastRun = DateTime.UtcNow;

        try
        {
            await Task.Run(new TaskContext(logger));
            LastRunSuccess = true;
        }
        catch
        {
            LastRunSuccess = false;
            throw;
        }
        Reinvalidate();
    }

    public ITask Task { get; }
    public TimeSpan? DetermininateNextRun()
    {
        if (!RunAt.HasValue)
        {
            RunAt = _reinvalidate();
        }

        if (RunAt.HasValue)
        {
            return RunAt - DateTime.UtcNow;
        }

        return null;
    }

    public TimeSpan DetermininateLastRun()
    {
        return DateTime.UtcNow - _lastRun;
    }

    public bool? LastRunSuccess { get; set; }
    public string IntervalText()
    {
        return "Dynamic";
    }
}