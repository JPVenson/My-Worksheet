using System;
using System.Threading.Tasks;

namespace MyWorksheet.Website.Server.Shared.TaskScheduling;

public interface ITaskRunner
{
    bool Check();
    Task Execute(ILogger logger);
    ITask Task { get; }
    TimeSpan? DetermininateNextRun();
    TimeSpan DetermininateLastRun();
    bool? LastRunSuccess { get; }

    string IntervalText();
}