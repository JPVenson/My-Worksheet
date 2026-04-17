using System;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;

namespace MyWorksheet.Website.Server.Shared.TaskScheduling.TaskRunners;

public class SingleTaskRunner : ITaskRunner
{
    public SingleTaskRunner(ITask task)
    {
        Task = task;
    }

    public bool Check()
    {
        return true;
    }

    public Task Execute(ILogger logger)
    {
        return Task.Run(new TaskContext(logger));
    }

    public ITask Task { get; }
    public TimeSpan? DetermininateNextRun()
    {
        return TimeSpan.Zero;
    }

    public TimeSpan DetermininateLastRun()
    {
        return TimeSpan.Zero;
    }

    public bool? LastRunSuccess { get; }
    public string IntervalText()
    {
        return "On Demand Single Run";
    }
}

public class OnDemandTaskRunner : ITaskRunner
{
    private DateTime _lastRun;

    public OnDemandTaskRunner(ITask task)
    {
        Task = task;
    }

    public bool Check()
    {
        return false;
    }

    public async Task Execute(ILogger logger)
    {
        try
        {
            await Task.Run(new TaskContext(logger));
            _lastRun = DateTime.UtcNow;
            LastRunSuccess = true;
        }
        catch
        {
            LastRunSuccess = false;
            throw;
        }
    }

    public ITask Task { get; }
    public TimeSpan? DetermininateNextRun()
    {
        return null;
    }

    public TimeSpan DetermininateLastRun()
    {
        return DateTime.UtcNow - _lastRun;
    }

    public bool? LastRunSuccess { get; set; }

    public string IntervalText()
    {
        return "On Demand";
    }
}