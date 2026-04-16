using System;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Shared.TaskScheduling;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Services.ScheduledTasks;

[ScheduleOnDemand]
public class FailTask : BaseTask
{
    public override void DoWork(TaskContext context)
    {
        throw new NotImplementedException();
    }

    public override string NamedTask { get; protected set; } = "Fail Task";

    public FailTask(IDbContextFactory<MyworksheetContext> dbContextFactory) : base(dbContextFactory)
    {
    }
}