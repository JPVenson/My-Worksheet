using System;
using System.Collections.Generic;
using MyWorksheet.Website.Server.Shared.TaskScheduling.TaskRunners;

namespace MyWorksheet.Website.Server.Shared.TaskScheduling;

public interface ISchedulerService
{
    TimeSpan NextInteration { get; }
    void Reinvalidate();
    void Start();
    void Stop();
    bool TryAddTask(ITask task, Schedule schedule);
    DynamicTaskRunner TryAddTask(ITask task, Func<DateTime> schedule);

    IReadOnlyCollection<ITaskRunner> Runners();
    public void RunOnceNow(ITask task);
}