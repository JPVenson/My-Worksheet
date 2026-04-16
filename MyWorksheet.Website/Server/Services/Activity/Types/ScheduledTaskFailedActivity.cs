using System;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Activity.Types;
using MyWorksheet.Website.Server.Shared.TaskScheduling;

namespace MyWorksheet.Website.Server.Services.Activity;

public class ScheduledTaskFailedActivity : ActivityType
{
    private readonly IActivityService _activityService;

    public ScheduledTaskFailedActivity(IActivityService activityService) : base("scheduled_task_failed")
    {
        _activityService = activityService;
    }

    public UserActivity CreateActivity(MyworksheetContext db, ITask task, string reasonShort)
    {
        return new UserActivity()
        {
            ActivityType = TypeKey,
            DateCreated = DateTime.UtcNow,
            SystemActivityTypeKey = Guid.NewGuid().ToString(),
            HeaderHtml = $"Task {task.NamedTask} failed",
            BodyHtml = $"Task with name {task.NamedTask} failed because: \r\n" + reasonShort,
            IdAppUser = Guid.Empty
        };
    }

    public async Task Create(MyworksheetContext db, ITask task, string exception)
    {
        await _activityService.CreateActivity(CreateActivity(db, task, exception));
    }
}