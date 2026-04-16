using System;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Activity;
using MyWorksheet.Website.Server.Shared.TaskScheduling;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Services.ScheduledTasks.Activity;

[ScheduleTaskAt(1, 0, 0, 0)]
public class CreateActivityForOldWorksheets : BaseTask
{
    public CreateActivityForOldWorksheets(IDbContextFactory<MyworksheetContext> factory) : base(factory)
    {

    }

    public override string NamedTask { get; protected set; } = "Create Reminder For Old Worksheets";

    public override async Task DoWorkAsync(TaskContext context)
    {
        var db = await DbContectFactory.CreateDbContextAsync().ConfigureAwait(false);
        var wsNotSubmitted = db.Worksheets.Where(f => f.IdCurrentStatus != Guid.Empty)
            .Where(e => e.EndTime < DateTime.UtcNow.AddDays(7))
            .ToArray();

        foreach (var ws in wsNotSubmitted)
        {
            await ActivityTypes.WorksheetNotSubmitted.CheckAndCreate(db, ws);
        }
    }
}