using System;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Activity;
using MyWorksheet.Website.Server.Shared.Services.Logger;
using MyWorksheet.Website.Server.Shared.TaskScheduling;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Services.ScheduledTasks;

public class DueActivityActivatorTask : BaseTask
{
    private readonly IActivityService _activityService;

    public DueActivityActivatorTask(IActivityService activityService, IDbContextFactory<MyworksheetContext> dbContextFactory) : base(dbContextFactory)
    {
        _activityService = activityService;
    }

    public override async Task DoWorkAsync(TaskContext context)
    {
        using var db = DbContectFactory.CreateDbContext();
        var dueActivities = db.UserActivities.Where(e => e.DueDate != null && e.DueDate < DateTime.UtcNow.AddMinutes(10))
            .Where(f => f.Activated == false)
            .ToArray();

        context.Logger.LogInformation($"Invoked now for {dueActivities.Length} due activities", LoggerCategories.ServerTask.ToString());

        foreach (var userActivity in dueActivities)
        {
            await _activityService.ActivateActivity(userActivity, db);
        }
    }

    public override string NamedTask { get; protected set; } =
        "Task that will be invoked when the next due Task should be activated";
}