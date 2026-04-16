using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Katana.CommonTasks.Models;
using MyWorksheet.Shared.WebApi;
using MyWorksheet.Webpage.Helper.Roles;
using MyWorksheet.Webpage.Services.WebHooks;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Budget;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Services.ObjectChanged;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using MyWorksheet.Website.Server.Util.Auth;
using MyWorksheet.Website.Server.Util.Extentions;
using MyWorksheet.Website.Server.Util.WorksheetItemUtil;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using MyWorksheet.Website.Shared.ViewModels.Notifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Controllers.Api.WorksheetApi;

[Route("api/WorksheetItemsApi")]
[RevokableAuthorize(Roles = Roles.AdminRoleName + "," + Roles.WorksheetAdminRoleName + "," +
                            Roles.WorksheetUserRoleName)]
public class WorksheetItemsApiControllerBase : RestApiControllerBase<WorksheetItem, WorksheetItemModel, WorksheetItemModel>
{
    public WorksheetItemsApiControllerBase(IMapperService mapper,
        IDbContextFactory<MyworksheetContext> entitiesFactory,
        WebHookService webHookService,
        IAppLogger logger,
        IBudgetService budgetService,
        ObjectChangedService objectChangedService) : base(entitiesFactory, mapper)
    {
        _webHookService = webHookService;
        _logger = logger;
        _budgetService = budgetService;
        _objectChangedService = objectChangedService;
    }

    private readonly WebHookService _webHookService;
    private readonly IAppLogger _logger;
    private readonly IBudgetService _budgetService;
    private readonly ObjectChangedService _objectChangedService;

    [HttpGet]
    [Route("GetWorksheetTexts")]
    public IActionResult GetWorksheetTexts(Guid worksheetId, string likeText)
    {
        using var db = EntitiesFactory.CreateDbContext();
        var worksheetItemComments = db
            .WorksheetComments
            .Where(e => e.WorksheetId == worksheetId)
            .Select(f => new { f.IdCreator, f.Comment })
            .Where(e => e.IdCreator == User.GetUserId() && e.Comment.Contains(likeText));
        // .Select<>(new object[] { likeText ?? string.Empty, User.GetUserId() });
        return Data(worksheetItemComments.Select(e => e.Comment).ToArray());
    }

    [HttpGet]
    [Route("GetByWorksheet")]
    public IActionResult GetWorksheetItemItems(Guid worksheetId)
    {
        using var db = EntitiesFactory.CreateDbContext();
        return Data(MapperService.ViewModelMapper.Map<WorksheetItemModel[]>(db.WorksheetItems.Where(f => f.IdCreator == User.GetUserId())
            .Where(f => f.IdWorksheet == worksheetId)
            .ToArray()));
    }

    [HttpGet]
    [Route("MedianStartingTime")]
    public IActionResult GetStartingTimeMedian(Guid projectId)
    {
        using var db = EntitiesFactory.CreateDbContext();

        return Data(0);

        // TODO: FIX
        // var lastXMonths = EntitiesFactory.CreateDbContext().RunSelect<int>(EntitiesFactory.CreateDbContext().Database
        // 	.CreateCommandWithParameterValues("SELECT [dbo].[WorksheetTimesMedian](@creator,@project,4)",
        // 		new QueryParameter("creator", User.GetUserId()),
        // 		new QueryParameter("project", projectId)));

        // return Data(lastXMonths);
    }

    [HttpGet]
    [Route("GetWorksheetItemItems")]
    public IActionResult GetWorksheetItemItems(Guid worksheetId, DateTimeOffset day)
    {
        using var db = EntitiesFactory.CreateDbContext();
        return Data(MapperService.ViewModelMapper.Map<WorksheetItemModel[]>(db.WorksheetItems.Where(f => f.IdCreator == User.GetUserId())
            .Where(f => f.IdWorksheet == worksheetId)
            .Where(f => f.DateOfAction == day.Date)
            .ToArray()));
    }

    [HttpGet]
    [Route("GetProjectWorksheetItemItems")]
    public IActionResult GetProjectWorksheetItemItems(Guid projectId, DateTimeOffset day)
    {
        var db = EntitiesFactory.CreateDbContext();
        var worksheets = db.Worksheets
            .Where(f => f.IdProject == projectId)
            .Where(f => f.IdCreator == User.GetUserId())
            .Where(e => (e.EndTime > day || e.EndTime == null) && e.StartTime < day)
            .Select(e => e.WorksheetId)
            .ToArray();

        if (!worksheets.Any())
        {
            return Data(Array.Empty<WorksheetItemModel>());
        }

        return Data(MapperService.ViewModelMapper.Map<WorksheetItemModel[]>(db.WorksheetItems.Where(f => f.IdCreator == User.GetUserId())
        .Where(e => worksheets.Contains(e.IdWorksheet))
            .Where(f => f.DateOfAction == day.Date)
            //.Column($"CONVERT(DATE, {nameof(WorksheetItem.DateOfAction)})").Is.EqualsTo(day.Date)
            .ToArray()));
    }

    [HttpPost]
    [Route("Replace")]
    public async Task<IActionResult> ReplaceItems([FromBody] WorksheetItemModel model, Guid[] toReplace)
    {
        if (model.IdWorksheet == Guid.Empty)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        if (CheckTrackerRunning(model.IdWorksheet))
        {
            return BadRequest("WorksheetItem/TrackerRunning".AsTranslation());
        }

        var worksheetItem = MapperService.ViewModelMapper.Map<WorksheetItem>(model);
        if (worksheetItem.IdWorksheet == Guid.Empty)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var db = EntitiesFactory.CreateDbContext();
        var ws = db.Worksheets.Where(e => e.IdCreator == User.GetUserId() && e.WorksheetId == worksheetItem.IdWorksheet)
            .FirstOrDefault();

        if (ws == null)
        {
            _logger.LogCritical("Prevented attempted id injection occured! Method: ReplaceItems.1", "Security",
                new Dictionary<string, string>()
                {
                    {
                        "virtualId", worksheetItem.IdWorksheet.ToString()
                    },
                    {
                        "UserId", User.GetUserId().ToString()
                    }
                });
            return Unauthorized("Common/InvalidId".AsTranslation());
        }
        var project = db.Projects.Find(ws.IdProject);
        if (project.IdOrganisation.HasValue)
        {
            var org = db.Organisations.Find(project.IdOrganisation.Value);
            if (org.IsDeleted || !org.IsActive)
            {
                return BadRequest("The organisation of this project is ether Inactive or deleted");
            }
        }
        worksheetItem.IdWorksheet = ws.WorksheetId;
        worksheetItem.IdCreator = User.GetUserId();

        var toDelete =
            db.WorksheetItems
                .Where(f => f.IdCreator == User.GetUserId() && toReplace.Contains(f.WorksheetItemId)).ToArray();

        if (toDelete.Length != toReplace.Length)
        {
            return BadRequest("Invalid Id Count");
        }

        if (toDelete.Any(f => f.IdWorksheet != worksheetItem.IdWorksheet))
        {
            _logger.LogCritical("Prevented attempted id injection occured! Method: ReplaceItems.2", "Security",
                new Dictionary<string, string>()
                {
                    {
                        "virtualId", worksheetItem.IdWorksheet.ToString()
                    },
                    {
                        "UserId", User.GetUserId().ToString()
                    }
                });
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        using var transaction = db.Database.BeginTransaction();
        var substract = toDelete.Select(e => e.ToTime - e.FromTime).Sum();

        var substractionResult = _budgetService.Substract(db, ws.IdProject, User.GetUserId(), substract);

        if (!substractionResult)
        {
            transaction.Rollback();
            return BadRequest(substractionResult.Reason);
        }

        var additionResult = _budgetService.Add(db, ws.IdProject, User.GetUserId(),
            worksheetItem.ToTime - worksheetItem.FromTime);

        if (!additionResult)
        {
            transaction.Rollback();
            return BadRequest(additionResult.Reason);
        }

        foreach (var worksheetItemToDelete in toDelete)
        {
            db.WorksheetItems.Remove(worksheetItemToDelete);
        }
        db.Add(worksheetItem);
        db.SaveChanges();
        transaction.Commit();


        await _objectChangedService.SendObjectChanged(ChangeEventTypes.Removed, toDelete, Request.GetSignalId(), User.GetUserId());
        await _objectChangedService.SendObjectChanged(ChangeEventTypes.Added, worksheetItem, Request.GetSignalId(), User.GetUserId());

        return Data(MapperService.ViewModelMapper.Map<WorksheetItemModel>(worksheetItem));
    }

    [NonAction]
    private bool CheckTrackerRunning(Guid worksheetItem)
    {
        using var db = EntitiesFactory.CreateDbContext();
        return EntitiesFactory.CreateDbContext().WorksheetTracks
            .Where(f => f.IdWorksheet == worksheetItem).Any();
    }

    public override async ValueTask<IActionResult> Create([FromBody] WorksheetItemModel model)
    {
        var worksheetItem = MapperService.ViewModelMapper.Map<WorksheetItem>(model);
        if (worksheetItem.IdWorksheet == Guid.Empty)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }
        if (CheckTrackerRunning(model.IdWorksheet))
        {
            return BadRequest("WorksheetItem/TrackerRunning".AsTranslation());
        }

        var db = EntitiesFactory.CreateDbContext();
        var ws = db.Worksheets.Where(e => e.IdCreator == User.GetUserId() && e.WorksheetId == worksheetItem.IdWorksheet)
            .FirstOrDefault();

        if (ws == null)
        {
            _logger.LogCritical("Prevented attempted id injection occured! Method: ReplaceItems.1", "Security",
                new Dictionary<string, string>()
                {
                    {
                        "virtualId", worksheetItem.IdWorksheet.ToString()
                    },
                    {
                        "UserId", User.GetUserId().ToString()
                    }
                });
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var project = db.Projects.Find(ws.IdProject);
        if (project.IdOrganisation.HasValue)
        {
            var org = db.Organisations.Find(project.IdOrganisation.Value);
            if (org.IsDeleted || !org.IsActive)
            {
                return BadRequest("The organisation of this project is ether Inactive or deleted");
            }
        }

        worksheetItem.IdWorksheet = ws.WorksheetId;
        worksheetItem.IdCreator = User.GetUserId();
        var mergeReport = MergeReporter.MergeReport(
            worksheetItem.DateOfAction.AddMinutes(worksheetItem.FromTime),
            worksheetItem.DateOfAction.AddMinutes(worksheetItem.ToTime),
            db,
            worksheetItem.IdWorksheet);
        if (mergeReport.Overlapping.Any())
        {
            return Conflict(mergeReport);
        }

        //if (!skipOverlapCheck)
        //{
        //	var ws = _db.Select<Worksheet>(realId);
        //	var hasOverlapping = _db.Worksheets.Where(f => f.Year == ws.Year).And
        //	                        .Column(f => f.Month).Is.EqualsTo(ws.Month)
        //	                        .Where(f => f.IdCreator == User.GetUserId())
        //	                        .ToArray();
        //	if (hasOverlapping.Any())
        //	{
        //		var overlappingTimeframe = _db.WorksheetItems
        //		                              .Where
        //		                              .Column(f => f.IdWorksheet)
        //		                              .Is
        //		                              .In(hasOverlapping.Select(e => e.WorksheetId).ToArray())
        //		                              .And
        //		                              .Column(f => f.DayInMonth)
        //		                              .Is
        //		                              .EqualsTo(model.DayInMonth)
        //		                              .And
        //		                              .Column(f => f.FromTime).Is.SmallerThen(model.ToTime)
        //		                              .And
        //		                              .Column(f => f.ToTime).Is.BiggerThen(model.FromTime)
        //		                              .FirstOrDefault();

        //		if (overlappingTimeframe != null)
        //		{
        //			return Conflict();
        //		}
        //	}
        //}

        var exceedsBudget = _budgetService.Add(db, ws.IdProject, User.GetUserId(),
            worksheetItem.ToTime - worksheetItem.FromTime);

        if (!exceedsBudget)
        {
            return BadRequest(exceedsBudget.Reason);
        }

        db.Add(worksheetItem);
        db.SaveChanges();
        var externalWebHookData = MapperService.ViewModelMapper.Map<WorksheetItemModel>(worksheetItem);

        _webHookService.PublishEvent(WebHookTypes.WorksheetItem,
            WebHookTypes.WorksheetItem.CreateHookObject(externalWebHookData, ActionTypes.Created),
            worksheetItem.IdCreator,
            Request.HttpContext.Connection.RemoteIpAddress.ToString());
        await _objectChangedService.SendObjectChanged(ChangeEventTypes.Added, worksheetItem, Request.GetSignalId(), User.GetUserId());

        //AccessElement<WorksheetItemHubInfo>.Instance.SendWorksheetItemChanged(worksheetItem.IdWorksheet, externalWebHookData, ActionTypes.Created);
        //AccessElement<ProjectHubInfo>.Instance.SendWorksheetItemChanged(project.ProjectId, externalWebHookData, ActionTypes.Created);
        return Data(externalWebHookData);
    }

    public override async ValueTask<IActionResult> Update([FromBody] WorksheetItemModel model, [FromQuery] Guid id)
    {
        var db = EntitiesFactory.CreateDbContext();
        var worksheetItem = db.WorksheetItems
            .Where(f => f.IdCreator == User.GetUserId())
            .Where(e => e.WorksheetItemId == id)
            .FirstOrDefault();

        if (worksheetItem == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        if (CheckTrackerRunning(model.IdWorksheet))
        {
            return BadRequest("WorksheetItem/TrackerRunning".AsTranslation());
        }

        var ws = db.Worksheets.Find(worksheetItem.IdWorksheet);
        var project = db.Projects.Find(ws.IdProject);
        if (project.IdOrganisation.HasValue)
        {
            var org = db.Organisations.Find(project.IdOrganisation.Value);
            if (org.IsDeleted || !org.IsActive)
            {
                return BadRequest("The organisation of this project is ether Inactive or deleted");
            }
        }

        var nTime = model.ToTime - model.FromTime;
        var oTime = worksheetItem.ToTime - model.FromTime;

        QuestionableBoolean result = true;
        if (nTime < oTime)
        {
            result =
                _budgetService.Substract(db, ws.IdProject, User.GetUserId(), oTime - nTime);
        }
        else if (nTime > oTime)
        {
            result =
                _budgetService.Add(db, ws.IdProject, User.GetUserId(), nTime - oTime);
        }

        if (!result)
        {
            return BadRequest(result.Reason);
        }

        worksheetItem.FromTime = model.FromTime;
        worksheetItem.ToTime = model.ToTime;
        worksheetItem.Comment = model.Comment;
        worksheetItem.IdProjectItemRate = model.IdProjectItemRate;

        db.Update(worksheetItem);
        db.SaveChanges();
        var externalWebHookData = MapperService.ViewModelMapper.Map<WorksheetItemModel>(worksheetItem);
        _webHookService.PublishEvent(WebHookTypes.WorksheetItem,
            WebHookTypes.WorksheetItem.CreateHookObject(externalWebHookData, ActionTypes.Updated), worksheetItem.IdCreator,
            Request.HttpContext.Connection.RemoteIpAddress.ToString());
        await _objectChangedService.SendObjectChanged(ChangeEventTypes.Changed, worksheetItem, Request.GetSignalId(), User.GetUserId());

        //AccessElement<WorksheetItemHubInfo>.Instance.SendWorksheetItemChanged(worksheetItem.IdWorksheet, externalWebHookData, ActionTypes.Updated);
        //AccessElement<ProjectHubInfo>.Instance.SendWorksheetItemChanged(project.ProjectId, externalWebHookData, ActionTypes.Updated);
        return Data(externalWebHookData);
    }

    [HttpGet]
    [Route("MergeReport")]
    public IActionResult MergeReport(DateTimeOffset from, DateTimeOffset to, Guid worksheetId)
    {
        using var db = EntitiesFactory.CreateDbContext();
        return Data(MapperService.ViewModelMapper.Map<MergeReportViewModel>(MergeReporter.MergeReport(from, to, db, worksheetId)));
    }

    [HttpPost]
    [Route("Delete")]
    public async Task<IActionResult> DeleteWorksheetItem(Guid id)
    {
        var db = EntitiesFactory.CreateDbContext();
        var worksheetItem = db.WorksheetItems
            .FirstOrDefault(f => f.IdCreator == User.GetUserId() && f.WorksheetItemId == id);

        if (worksheetItem == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        if (CheckTrackerRunning(worksheetItem.IdWorksheet))
        {
            return BadRequest("WorksheetItem/TrackerRunning".AsTranslation());
        }

        var ws = db.Worksheets.Find(worksheetItem.IdWorksheet);
        var project = db.Projects.Find(ws.IdProject);
        if (project.IdOrganisation.HasValue)
        {
            var org = db.Organisations.Find(project.IdOrganisation.Value);
            if (org.IsDeleted || !org.IsActive)
            {
                return BadRequest("The organisation of this project is ether Inactive or deleted");
            }
        }
        var result = _budgetService.Substract(db, ws.IdProject, User.GetUserId(),
            worksheetItem.ToTime - worksheetItem.FromTime);

        if (!result)
        {
            return BadRequest(result.Reason);
        }

        db.WorksheetItems.Remove(worksheetItem);
        db.SaveChanges();
        var externalWebHookData = MapperService.ViewModelMapper.Map<WorksheetItemModel>(worksheetItem);
        _webHookService.PublishEvent(WebHookTypes.WorksheetItem,
            WebHookTypes.WorksheetItem.CreateHookObject(externalWebHookData, ActionTypes.Deleted),
            worksheetItem.IdCreator,
            Request.HttpContext.Connection.RemoteIpAddress.ToString());
        await _objectChangedService.SendObjectChanged(ChangeEventTypes.Removed, worksheetItem, Request.GetSignalId(), User.GetUserId());
        //AccessElement<WorksheetItemHubInfo>.Instance.SendWorksheetItemChanged(worksheetItem.IdWorksheet, externalWebHookData, ActionTypes.Deleted);
        //AccessElement<ProjectHubInfo>.Instance.SendWorksheetItemChanged(project.ProjectId, externalWebHookData, ActionTypes.Deleted);
        return Data();
    }
}