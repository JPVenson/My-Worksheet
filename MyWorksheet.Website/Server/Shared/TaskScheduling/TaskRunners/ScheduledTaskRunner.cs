using System;
using System.Threading.Tasks;
using Humanizer;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;

namespace MyWorksheet.Website.Server.Shared.TaskScheduling.TaskRunners;

internal class ScheduledTaskRunner : ITaskRunner
{
    private DateTime _lastRun;

    public ScheduledTaskRunner(ITask task, TimeSpan runAt)
    {
        Task = task;
        RunAt = runAt;

        if (runAt < DateTime.UtcNow.TimeOfDay)
        {
            _lastRun = DateTime.UtcNow;
        }
    }

    public ITask Task { get; }

    public TimeSpan? DetermininateNextRun()
    {
        var now = DateTime.UtcNow;
        if (_lastRun.Date == DateTime.UtcNow.Date)
        {
            var nextRun = RunAt.Add(new TimeSpan(1, 0, 0, 0)).Subtract(now.TimeOfDay);
            return nextRun;
        }

        return RunAt - now.TimeOfDay;
    }

    public TimeSpan DetermininateLastRun()
    {
        return DateTime.UtcNow - _lastRun;
    }

    public bool? LastRunSuccess { get; private set; }
    public string IntervalText()
    {
        return $"At {RunAt.Humanize()}";
    }

    protected TimeSpan RunAt { get; }

    public async Task Execute(IAppLogger logger)
    {
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
        finally
        {
            _lastRun = DateTime.UtcNow;
        }
    }

    public bool Check()
    {
        return _lastRun.Date != DateTime.UtcNow.Date && DateTime.UtcNow.TimeOfDay > RunAt && !Task.IsBusy;
    }
}