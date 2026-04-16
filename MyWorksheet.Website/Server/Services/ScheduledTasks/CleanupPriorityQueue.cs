using System;
using System.Linq;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.UserCounter;
using MyWorksheet.Website.Server.Shared.TaskScheduling;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Services.ScheduledTasks;

[ScheduleTaskEvery(1, 0, 0, 0)]
public class CleanupPriorityQueue : BaseTask
{
    private readonly IUserQuotaService _userQuotaService;

    public CleanupPriorityQueue(IUserQuotaService userQuotaService, IDbContextFactory<MyworksheetContext> dbContextFactory) : base(dbContextFactory)
    {
        _userQuotaService = userQuotaService;
    }

    /// <inheritdoc />
    public override void DoWork(TaskContext context)
    {
        using var db = DbContectFactory.CreateDbContext();
        var query = db.PriorityQueueItems
            .Where(f => f.Done && f.Error == null && f.DateOfDone < DateTime.UtcNow.AddDays(-7));

        var totalItems = query.Count();

        context.Logger.LogInformation($"Delete {totalItems} Successful Priority items");

        var totalDeleted = 0;
        do
        {
            var priorityQueueItems = query.Take(50).ToArray();
            if (priorityQueueItems.Length == 0)
            {
                break;
            }
            var values = priorityQueueItems.Select(f => f.PriorityQueueItemId).ToArray();
            totalDeleted += query.Take(50).ExecuteDelete();
            context.Logger.LogInformation($"Deleted batch of {values.Length} - {totalItems} remaining: {totalItems - totalDeleted} items");
        } while (true);
    }

    /// <inheritdoc />
    public override string NamedTask { get; protected set; } = "Purge Successfull Tasks";
}