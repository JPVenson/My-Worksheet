using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Activity;
using MyWorksheet.Website.Server.Shared.TaskScheduling;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Services.ScheduledTasks.Activity;

[ScheduleTaskEvery(1, 0, 0)]
public class NotifyUserForRunningTrackingTask : BaseTask
{
    public NotifyUserForRunningTrackingTask(IDbContextFactory<MyworksheetContext> dbContextFactory) : base(dbContextFactory)
    {
    }

    public override async Task DoWorkAsync(TaskContext context)
    {
        using var db = DbContectFactory.CreateDbContext();
        var tracker = db.WorksheetTracks.ToArray();
        foreach (var track in tracker)
        {
            await ActivityTypes.TrackerStillRunning.CheckAndCreate(db, track);
        }
    }

    public override string NamedTask { get; protected set; } = "Notify Users for Still running a Tracker";

}