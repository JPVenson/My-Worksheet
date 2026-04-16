using System;
using System.Linq;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Shared.TaskScheduling;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Services.ScheduledTasks;

[ScheduleTaskAt(0, 0, 0)]
public class CleanupWebhookHistoryTask : BaseTask
{
    public CleanupWebhookHistoryTask(IDbContextFactory<MyworksheetContext> dbContextFactory) : base(dbContextFactory)
    {
    }

    public override void DoWork(TaskContext context)
    {
        using var db = DbContectFactory.CreateDbContext();
        db.OutgoingWebhookActionLogs.Where(e => e.DateOfAction < DateTime.UtcNow.AddDays(-30)).ExecuteDelete();
    }

    public override string NamedTask { get; protected set; } = "Cleanup Webhook History";
}