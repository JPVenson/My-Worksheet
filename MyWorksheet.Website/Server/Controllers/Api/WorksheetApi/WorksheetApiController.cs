using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Shared.WebApi;
using MyWorksheet.Webpage.Helper;
using MyWorksheet.Webpage.Helper.Roles;
using MyWorksheet.Webpage.Services.WebHooks;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Blob;
using MyWorksheet.Website.Server.Services.Budget;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Services.NumberRangeService;
using MyWorksheet.Website.Server.Services.ObjectChanged;
using MyWorksheet.Website.Server.Services.UserCounter;
using MyWorksheet.Website.Server.Services.WorksheetWorkflowManagment;
using MyWorksheet.Website.Server.Shared.Services.Logger;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using MyWorksheet.Website.Server.Util.Auth;
using MyWorksheet.Website.Server.Util.Extentions;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using MyWorksheet.Website.Shared.ViewModels.Notifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Controllers.Api.WorksheetApi;

[Route("api/WorksheetApi")]
[RevokableAuthorize(Roles =
    Roles.AdminRoleName + "," + Roles.WorksheetAdminRoleName + "," + Roles.WorksheetUserRoleName)]
public class WorksheetApiControllerBase : RestApiControllerBase<Worksheet, WorksheetModel, WorksheetModel>
{
    private readonly IAppLogger _logger;
    private readonly WorksheetWorkflowManager _workflowManager;
    private readonly IUserQuotaService _userQuotaService;
    private readonly INumberRangeService _numberRangeService;
    private readonly ObjectChangedService _objectChangedService;
    private readonly WebHookService _webHookService;
    private IBudgetService _budgetService;
    private IBlobManagerService _blobManagerService;

    public WorksheetApiControllerBase(IMapperService mapper,
        WebHookService webHookService,
        IAppLogger logger,
        WorksheetWorkflowManager workflowManager,
        IUserQuotaService userQuotaService,
        IDbContextFactory<MyworksheetContext> dbContextFactory,
        INumberRangeService numberRangeService,
        ObjectChangedService objectChangedService) : base(dbContextFactory, mapper)
    {
        _webHookService = webHookService;
        _logger = logger;
        _workflowManager = workflowManager;
        _userQuotaService = userQuotaService;
        _numberRangeService = numberRangeService;
        _objectChangedService = objectChangedService;
    }

    public Worksheet GetWorksheetByNo(Guid userId, Guid virtualId)
    {
        var realId = EntitiesFactory.CreateDbContext().Worksheets.Where(f => f.WorksheetId == virtualId)
            .Where(f => f.IdCreator == userId)
            .FirstOrDefault();

        if (realId == null)
        {
            _logger.LogCritical("Prevented attempted id injection occured! Method: GetWorksheetByNo.1", "Security",
                new Dictionary<string, string>
                {
                    {
                        "virtualId", virtualId.ToString()
                    },
                    {
                        "UserId", userId.ToString()
                    }
                });
            return null;
        }

        return realId;
    }

    [HttpGet]
    [Route("Worksheets")]
    public IActionResult GetWorksheetItems(Guid projectId, bool? showHidden = null)
    {
        var query = EntitiesFactory.CreateDbContext().Worksheets
            .Where(f => f.IdCreator == User.GetUserId())
            .Where(f => f.IdProject == projectId);
        if (showHidden.HasValue)
        {
            query = query
                .Where(f => f.Hidden == showHidden);
        }

        var result = query.ToArray();
        return Data(MapperService.ViewModelMapper.Map<WorksheetModel[]>(result));
    }

    public override async ValueTask<IActionResult> Update(WorksheetModel model, Guid id)
    {
        var worksheet = GetWorksheetByNo(User.GetUserId(), model.WorksheetId);

        if (worksheet == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        if (worksheet.IdWorksheetWorkflow.HasValue &&
            !_workflowManager.GetCanModify(worksheet.IdWorksheetWorkflow.Value, worksheet.IdCurrentStatus))
        {
            return BadRequest("The worksheet is Submitted");
        }

        var db = EntitiesFactory.CreateDbContext();
        db.Update(worksheet);
        var externalWebHookData = MapperService.ViewModelMapper.Map<WorksheetModel>(worksheet);
        _webHookService.PublishEvent(WebHookTypes.Worksheet,
            WebHookTypes.Worksheet.CreateHookObject(externalWebHookData, ActionTypes.Updated),
            worksheet.IdCreator,
            Request.HttpContext.Connection.RemoteIpAddress.ToString());
        await _objectChangedService.SendObjectChanged(ChangeEventTypes.Changed, worksheet, Request.GetSignalId(), User.GetUserId());
        return Data();
    }

    public override async ValueTask<IActionResult> Create(WorksheetModel model)
    {
        if (!(await _userQuotaService.Add(User.GetUserId(), 1, Quotas.Worksheet)))
        {
            _logger.LogInformation("Exceeded Project Quota", LoggerCategories.Worksheet.ToString());
            return BadRequest("You have Exceeded the max number of Worksheets");
        }

        var db = EntitiesFactory.CreateDbContext();

        var worksheet = MapperService.ViewModelMapper.Map<Worksheet>(model);

        if (worksheet.EndTime.HasValue && worksheet.StartTime > worksheet.EndTime)
        {
            return BadRequest("Start date must be smaller then the End date");
        }

        if (worksheet.IdProject == Guid.Empty)
        {
            return BadRequest("Invalid Project Id");
        }
        worksheet.IdCreator = User.GetUserId();

        var project = db.Projects
            .Include(e => e.IdWorksheetWorkflowNavigation)
            .Where(e => e.ProjectId == model.IdProject)
            .FirstOrDefault();

        if (project == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        if (project.IdOrganisation.HasValue)
        {
            var organisation = db.Organisations
                .Where(e => e.OrganisationId == project.IdOrganisation.Value)
                .FirstOrDefault();

            if (organisation == null)
            {
                return Unauthorized("Common/InvalidId".AsTranslation());
            }

            if (organisation.IsDeleted || !organisation.IsActive)
            {
                return BadRequest("The assosiated Organisation is Inactive or was deleted.");
            }
        }

        worksheet.IdWorksheetWorkflow = project.IdWorksheetWorkflow;
        worksheet.IdWorksheetWorkflowDataMap = project.IdWorksheetWorkflowDataMap;
        // worksheet.IdCurrentStatus = project.IdWorksheetWorkflowNavigation?.IdDefaultStep; //TODO check if that actually works
        worksheet.IdProject = project.ProjectId;

        var transaction = await db.Database.BeginTransactionAsync();
        await using (transaction.ConfigureAwait(false))
        {
            worksheet.NumberRangeEntry = await _numberRangeService.GetNumberRangeAsync(db, WorksheetNumberRangeFactory.NrCode, worksheet.IdCreator, model);
            db.Add(worksheet);
            db.SaveChanges();

            var questionableBoolean =
                await _workflowManager.SetWorksheetWorkflowStep(db, worksheet,
                    _workflowManager.GetDefaultStepFor(worksheet.IdWorksheetWorkflow ?? Guid.Empty),
                    User.GetUserId(), "Creation", new Dictionary<string, object>());
            if (!questionableBoolean)
            {
                transaction.Rollback();
                return BadRequest(questionableBoolean.Reason);
            }
            db.SaveChanges();
            transaction.Commit();
        }

        var externalWebHookData = MapperService.ViewModelMapper.Map<WorksheetModel>(worksheet);
        _webHookService.PublishEvent(WebHookTypes.Worksheet,
            WebHookTypes.Worksheet.CreateHookObject(externalWebHookData, ActionTypes.Created), User.GetUserId(),
            Request.HttpContext.Connection.RemoteIpAddress.ToString());
        await _objectChangedService.SendObjectChanged(ChangeEventTypes.Added, worksheet, Request.GetSignalId(), User.GetUserId());
        return Data(externalWebHookData);
    }

    [HttpPost]
    [Route("Delete")]
    public async Task<IActionResult> DeleteWorksheet(Guid id)
    {
        var worksheet = GetWorksheetByNo(User.GetUserId(), id);

        if (worksheet == null)
        {
            return BadRequest("Wrong Id");
        }

        var db = EntitiesFactory.CreateDbContext();
        if (worksheet.IdWorksheetWorkflow.HasValue)
        {
            if (!_workflowManager.GetCanModify(worksheet.IdWorksheetWorkflow.Value, worksheet.IdCurrentStatus))
            {
                worksheet.Hidden = !worksheet.Hidden;
                db.Update(worksheet);
                return Data();
            }
        }

        await WorksheetHelper.DeleteWorksheet(db, id, User.GetUserId(), _budgetService, _blobManagerService);

        var externalWebHookData = MapperService.ViewModelMapper.Map<WorksheetModel>(worksheet);
        _webHookService.PublishEvent(WebHookTypes.Worksheet,
            WebHookTypes.Worksheet.CreateHookObject(externalWebHookData, ActionTypes.Submitted),
            worksheet.IdCreator,
            Request.HttpContext.Connection.RemoteIpAddress.ToString());
        db.Worksheets.Remove(worksheet);
        db.SaveChanges();
        await _userQuotaService.Subtract(User.GetUserId(), 1, Quotas.Worksheet);
        await _objectChangedService.SendObjectChanged(ChangeEventTypes.Removed, worksheet, Request.GetSignalId(), User.GetUserId());

        return Data();
    }
}