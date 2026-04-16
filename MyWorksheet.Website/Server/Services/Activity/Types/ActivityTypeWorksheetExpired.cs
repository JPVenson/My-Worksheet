using System;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;

namespace MyWorksheet.Website.Server.Services.Activity.Types;

public class ActivityTypeWorksheetExpired : ActivityType
{
    private readonly IActivityService _activityService;

    public string FormatKey(Worksheet ws)
    {
        return ws.IdCreator + "_" + ws.WorksheetId;
    }

    public UserActivity CreateActivity(MyworksheetContext db, Worksheet worksheet)
    {
        var proj = db.Projects.Find(worksheet.IdProject);
        if (!worksheet.EndTime.HasValue)
        {
            return null;
        }

        var endTime = worksheet.EndTime.Value;
        return new UserActivity()
        {
            ActivityType = TypeKey,
            DateCreated = DateTime.UtcNow,
            SystemActivityTypeKey = FormatKey(worksheet),
            HeaderHtml = "Worksheet " + proj.Name + ", " + (worksheet.NumberRangeEntry) + " Should be submitted soon",
            BodyHtml = "The Worksheet was set to " + (endTime.Month + 1 + "/" + endTime.Year) + " and has exceeded its livetime. It should be submitted soon",
            FooterHtml = "{{goto:Links/Timeboard.Worksheet:WorksheetId=" + worksheet.WorksheetId + "}}",
            IdAppUser = worksheet.IdCreator
        };
    }

    public async Task<bool> CheckAndCreate(MyworksheetContext db, Worksheet ws)
    {
        if (!ws.EndTime.HasValue)
        {
            return false;
        }

        var hasActivity = db.UserActivities.Where(f => f.IdAppUser == ws.IdCreator)
            .Where(e => e.SystemActivityTypeKey == ActivityTypes.WorksheetNotSubmitted.FormatKey(ws))
            .Where(f => f.ActivityType == ActivityTypes.WorksheetNotSubmitted.TypeKey)
            .FirstOrDefault();
        if (hasActivity == null)
        {
            await _activityService.CreateActivity(CreateActivity(db, ws));
            return true;
        }
        return false;
    }

    public ActivityTypeWorksheetExpired(IActivityService activityService) : base("activity_worksheet_not_submitted")
    {
        _activityService = activityService;
    }
}