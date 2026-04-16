using System;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Helper.Db;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Activity.Types;
using MyWorksheet.Website.Server.Settings;
using MyWorksheet.Website.Server.Services.Worktime;
using Microsoft.Extensions.Options;

namespace MyWorksheet.Website.Server.Services.Activity;

public class TrackerStillRunning : ActivityType
{
    private readonly IOptions<ActivitySettings> _activitySettings;
    private readonly IUserWorktimeService _userWorktimeService;
    private readonly IActivityService _activityService;

    public TrackerStillRunning(IOptions<ActivitySettings> activitySettings,
        IUserWorktimeService userWorktimeService,
        IActivityService activityService) : base("worksheet_tracker_stillRunning")
    {
        _activitySettings = activitySettings;
        _userWorktimeService = userWorktimeService;
        _activityService = activityService;
    }

    public string FormatKey(WorksheetTrack ws)
    {
        return ws.IdAppUser + ":" + ws.IdWorksheet;
    }

    public UserActivity CreateActivity(MyworksheetContext db, WorksheetTrack counter, Project project, Worksheet worksheet)
    {
        return new UserActivity()
        {
            ActivityType = TypeKey,
            DateCreated = DateTime.UtcNow,
            SystemActivityTypeKey = FormatKey(counter),
            HeaderHtml = "Worksheet Tracker is Running longer then Usual",
            BodyHtml = "The Worksheet Tracker in Project " + project.Name + " for worksheet " + worksheet.StartTime.Month + "/" + worksheet.StartTime.Year + " is now running for " + (DateTime.UtcNow - counter.DateStarted).Humanise(true),
            FooterHtml = "{{goto:Links/Timeboard.Worksheet:WorksheetId=" + worksheet.WorksheetId + "}}",
            IdAppUser = counter.IdAppUser
        };
    }

    public async Task<bool> CheckAndCreate(MyworksheetContext db, WorksheetTrack wst)
    {
        var ws = db.Worksheets.Find(wst.IdWorksheet);
        var proj = db.Projects.Find(ws.IdProject);

        var fallbackMeanWorktime = _activitySettings.Value.TrackerStillRunning.FallbackMwt;

        var projMeanWorktime = _userWorktimeService
            .GetWorkloadForProject(db, proj.ProjectId, wst.IdAppUser)
            .GetMeanWorktimeForDay(DateTime.UtcNow);
        var meanWorktime = TimeSpan.FromMinutes((double)(projMeanWorktime != -1 ? projMeanWorktime : fallbackMeanWorktime));

        if (DateTime.UtcNow - wst.DateStarted < meanWorktime)
        {
            return false;
        }

        var firstOrDefault = db.UserActivities.Where(f => f.IdAppUser == wst.IdAppUser)
            .Where(f => f.ActivityType == TypeKey)
            .Where(f => f.SystemActivityTypeKey == FormatKey(wst))
            .FirstOrDefault();

        if (firstOrDefault == null)
        {
            await _activityService.CreateActivity(CreateActivity(db, wst, proj, ws));
            return true;
        }
        return false;
    }
}