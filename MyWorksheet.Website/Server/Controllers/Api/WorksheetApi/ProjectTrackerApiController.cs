using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Shared.WebApi;
using MyWorksheet.Webpage.Helper.Roles;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Services.ObjectChanged;
using MyWorksheet.Website.Server.Util.Auth;
using MyWorksheet.Website.Server.Util.Extentions;
using MyWorksheet.Website.Server.Util.WorksheetItemUtil;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Controllers.Api.WorksheetApi;

[Route("api/ProjectTrackerApi")]
[RevokableAuthorize(Roles = Roles.AdminRoleName + "," + Roles.WorksheetAdminRoleName + "," + Roles.WorksheetUserRoleName)]
public class ProjectTrackerApiController : ApiControllerBase
{
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;
    private readonly IMapperService _mapperService;
    private readonly ObjectChangedService _objectChangedService;

    public ProjectTrackerApiController(IDbContextFactory<MyworksheetContext> dbContextFactory,
        IMapperService mapperService,
        ObjectChangedService objectChangedService)
    {
        _dbContextFactory = dbContextFactory;
        _mapperService = mapperService;
        _objectChangedService = objectChangedService;
    }

    [NonAction]
    private WorksheetTrack GetTrackItem(Guid worksheetId)
    {
        var db = _dbContextFactory.CreateDbContext();
        return db.WorksheetTracks.Include(e => e.IdWorksheetNavigation)
            .Where(e => e.IdWorksheet == worksheetId && e.IdAppUser == User.GetUserId())
            .FirstOrDefault();
    }

    [HttpPost]
    [Route("BeginTrack")]
    public async Task<IActionResult> BeginTrack(Guid worksheetId, Guid projectItemRateId)
    {
        var db = _dbContextFactory.CreateDbContext();

        var hasTrack = GetTrackItem(worksheetId);
        if (hasTrack != null)
        {
            return BadRequest("Tracking item Exists");
        }
        var worksheet = db.Worksheets.Where(e => e.IdCreator == User.GetUserId() && e.WorksheetId == worksheetId)
            .FirstOrDefault();
        if (worksheet == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }
        var newTrack = new WorksheetTrack();
        var dateTime = DateTimeOffset.UtcNow;
        var trackStarted = dateTime.AddSeconds(dateTime.Second * -1);
        newTrack.DateStartedOffset = (short)trackStarted.Offset.TotalMinutes;
        newTrack.DateStarted = trackStarted.ToUniversalTime();
        newTrack.IdAppUser = User.GetUserId();
        newTrack.IdProjectItemRate = projectItemRateId;
        newTrack.IdWorksheet = worksheet.WorksheetId;
        db.Add(newTrack);
        db.SaveChanges();
        var trackModel = _mapperService.ViewModelMapper.Map<WorksheetTimeTrackerViewModel>(newTrack);
        await _objectChangedService.SendObjectChanged(ChangeEventTypes.Added, newTrack, Request.GetSignalId(), User.GetUserId());
        trackModel.IdProject = worksheet.IdProject;
        return Data(trackModel);
    }

    [HttpGet]
    [Route("GetTrackers")]
    public IActionResult GetTrack()
    {
        var db = _dbContextFactory.CreateDbContext();
        var tracker = db.WorksheetTracks
            .Include(e => e.IdWorksheetNavigation)
            .Where(e => e.IdAppUser == User.GetUserId())
            .ToArray();
        return Data(_mapperService.ViewModelMapper.Map<WorksheetTimeTrackerViewModel[]>(tracker));
    }

    [HttpPost]
    [Route("UpdateTrack")]
    public async Task<IActionResult> UpdateTrack(Guid worksheetId, string comment)
    {
        var timesheetTrackStart = GetTrackItem(worksheetId);
        if (timesheetTrackStart != null)
        {
            var db = _dbContextFactory.CreateDbContext();
            db.WorksheetTracks.Where(e => e.WorksheetTrackId == timesheetTrackStart.WorksheetTrackId)
                .ExecuteUpdate(e => e.SetProperty(f => f.Comment, comment));

            await _objectChangedService.SendObjectChanged(ChangeEventTypes.Changed, timesheetTrackStart, Request.GetSignalId(),
                User.GetUserId());
        }

        return Data();
    }

    [HttpPost]
    [Route("GetTracker")]
    public IActionResult GetTrack(Guid worksheetId)
    {
        var timesheetTrackStart = GetTrackItem(worksheetId);
        if (timesheetTrackStart != null)
        {
            return Data(_mapperService.ViewModelMapper.Map<WorksheetTimeTrackerViewModel>(timesheetTrackStart));
        }

        return Data(null);
    }

    [HttpPost]
    [Route("AbortTrack")]
    public async Task<IActionResult> AbortTrack([FromQuery] Guid worksheetId)
    {
        var hasTrack = GetTrackItem(worksheetId);
        if (hasTrack == null)
        {
            return BadRequest("WorksheetItem/Tracker.DoesNotExist".AsTranslation());
        }
        var db = _dbContextFactory.CreateDbContext();
        db.WorksheetTracks.Remove(hasTrack);
        db.SaveChanges();
        await _objectChangedService.SendObjectChanged(ChangeEventTypes.Removed, typeof(WorksheetTrack), hasTrack.WorksheetTrackId, Request.GetSignalId(),
            User.GetUserId());
        return Data();
    }

    [HttpPost]
    [Route("EndTrack")]
    public async Task<IActionResult> EndTrack([FromQuery] Guid worksheetId, [FromQuery] DateTimeOffset? fromTime, [FromQuery] DateTimeOffset? toTime, [FromQuery] string comment)
    {
        var hasTrack = GetTrackItem(worksheetId);
        if (hasTrack == null)
        {
            return BadRequest("WorksheetItem/Tracker.DoesNotExist".AsTranslation());
        }

        var trackModel = _mapperService.ViewModelMapper.Map<WorksheetTimeTrackerViewModel>(hasTrack);

        if (fromTime.HasValue)
        {
            trackModel.StartTime = fromTime.Value;
        }

        if (toTime.HasValue)
        {
            trackModel.EndTime = toTime.Value;
        }

        if (trackModel.EndTime <= trackModel.StartTime)
        {
            return BadRequest("You requested an TrackEnd time that is smaller than your start time. are you drunk?");
        }

        var db = _dbContextFactory.CreateDbContext();
        var mergeReport = MergeReporter.MergeReport(hasTrack.DateStarted, trackModel.EndTime, db, worksheetId);
        if (mergeReport.Overlapping.Any())
        {
            return Data(new WorksheetTrackResult()
            {
                Success = false,
                MergeReportViewModel = _mapperService.ViewModelMapper.Map<MergeReportViewModel>(mergeReport)
            });
        }

        if (comment != null)
        {
            trackModel.Comment = comment;
        }

        var toAdds = new List<WorksheetItem>();
        using var transaction = db.Database.BeginTransaction();
        var timeDifSpan = trackModel.EndTime - trackModel.StartTime;
        var dates = (int)Math.Ceiling(timeDifSpan.TotalDays);
        for (int i = 0; i < dates; i++)
        {
            var toAdd = new WorksheetItem()
            {
                Comment = trackModel.Comment,
                DateOfActionOffset = (short)trackModel.StartTime.Offset.TotalMinutes,
                DateOfAction = new DateTimeOffset(trackModel.StartTime.Date.AddDays(i), trackModel.StartTime.Offset).ToUniversalTime(),
                IdProjectItemRate = trackModel.IdProjectItemRate,
                IdWorksheet = trackModel.IdWorksheet,
                IdCreator = User.GetUserId(),
            };
            if (i == 0)
            {
                toAdd.FromTime = trackModel.StartTime.Hour * 60 + trackModel.StartTime.Minute;
            }
            else
            {
                toAdd.FromTime = 0;
            }

            if (dates > 1 && i != dates)
            {
                toAdd.ToTime = (24 * 60) - 1;
            }
            else
            {
                toAdd.ToTime = trackModel.EndTime.Hour * 60 + trackModel.EndTime.Minute;
            }
            db.Add(toAdd);
            toAdds.Add(toAdd);
        }

        db.WorksheetTracks.Remove(hasTrack);
        db.SaveChanges();
        transaction.Commit();

        await _objectChangedService.SendObjectChanged(ChangeEventTypes.Removed, hasTrack, Request.GetSignalId(), User.GetUserId());
        await _objectChangedService.SendObjectChanged(ChangeEventTypes.Added, toAdds, Request.GetSignalId(), User.GetUserId());
        return Data(trackModel);
    }
}