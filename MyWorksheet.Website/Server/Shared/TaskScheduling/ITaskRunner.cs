using System;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;

namespace MyWorksheet.Website.Server.Shared.TaskScheduling;

public interface ITaskRunner
{
    bool Check();
    Task Execute(IAppLogger logger);
    ITask Task { get; }
    TimeSpan? DetermininateNextRun();
    TimeSpan DetermininateLastRun();
    bool? LastRunSuccess { get; }

    string IntervalText();
}