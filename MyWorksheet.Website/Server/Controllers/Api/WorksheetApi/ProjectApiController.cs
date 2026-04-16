using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using MyWorksheet.Shared.WebApi;
using MyWorksheet.Webpage.Helper;
using MyWorksheet.Webpage.Helper.Roles;
using MyWorksheet.Webpage.Services.WebHooks;
using MyWorksheet.Website.Server.Helper;
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
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using MyWorksheet.Website.Shared.ViewModels.Notifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Controllers.Api.WorksheetApi;

[Route("api/ProjectApi")]
[RevokableAuthorize(Roles = Roles.AdminRoleName + "," + Roles.WorksheetAdminRoleName + "," + Roles.WorksheetUserRoleName)]
public class ProjectControllerBase : RestApiControllerBase<Project, GetProjectModel, PostProjectApiModel>
{
    private readonly IBlobManagerService _blobManagerService;
    private readonly IBudgetService _budgetService;
    private readonly IAppLogger _logger;
    private readonly INumberRangeService _numberRangeService;
    private readonly ObjectChangedService _objectChangedService;
    private readonly IUserQuotaService _userQuotaService;
    private readonly WebHookService _webHooks;
    private readonly WorksheetWorkflowManager _worksheetWorkflowManager;

    public ProjectControllerBase(
        IMapperService mapper,
        IAppLogger logger,
        WebHookService webHooks,
        IUserQuotaService userQuotaService,
        WorksheetWorkflowManager worksheetWorkflowManager,
        IBudgetService budgetService,
        IBlobManagerService blobManagerService,
        IDbContextFactory<MyworksheetContext> dbContextFactory,
        ObjectChangedService objectChangedService,
        INumberRangeService numberRangeService
    )
        : base(dbContextFactory, mapper)
    {
        _logger = logger;
        _webHooks = webHooks;
        _userQuotaService = userQuotaService;
        _worksheetWorkflowManager = worksheetWorkflowManager;
        _budgetService = budgetService;
        _blobManagerService = blobManagerService;
        _objectChangedService = objectChangedService;
        _numberRangeService = numberRangeService;
    }

    protected override Project[] GetAllByUser()
    {
        var projectsInOrganisationViews = ListProjects(null);
        return MapperService.ViewModelMapper.Map<Project[]>(projectsInOrganisationViews);
    }

    [HttpGet]
    [Route("Search")]
    public IActionResult Search(string name, bool hidden)
    {
        using var db = EntitiesFactory.CreateDbContext();

        var query = db.Projects.Include(e => e.IdOrganisationNavigation)
            .Where(e => e.Hidden == !hidden)
            .Where(e => e.IdOrganisationNavigation.IsActive == !hidden)
            .Where(e => e.IdCreator == User.GetUserId())
            .Where(e => e.Name.Contains(name) || e.NumberRangeEntry.Contains(name) || e.ProjectReference.Contains(name));

        return Data(MapperService.ViewModelMapper.Map<GetProjectModel[]>(query));
    }

    [HttpGet]
    [Route("GetWorksheetItemsInRange")]
    public IActionResult GetWorksheetItemsInRange(Guid projectId, DateTimeOffset? startTime, DateTimeOffset? endTime)
    {
        using var db = EntitiesFactory.CreateDbContext();

        var projectsInOrganisations = ProjectsInOrganisation.Query(db, User.GetUserId())
                    .Where(e => (e.IdOrganisation == null && e.IdCreator == User.GetUserId())
                                || (e.IdOrganisation != null && e.IdAppUser == User.GetUserId()))
                                .Where(e => e.ProjectId == projectId)
                                .FirstOrDefault();

        if (projectsInOrganisations == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var worksheets = db.Worksheets.Where(e => e.IdProject == projectsInOrganisations.ProjectId)
        .Select(e => e.WorksheetId)
                                    .ToArray();

        if (!worksheets.Any())
        {
            return Data();
        }

        var query = db.WorksheetItems.Where(f => worksheets.Contains(f.IdWorksheet));

        if (startTime.HasValue)
        {
            query = query.Where(e => e.DateOfAction >= startTime.Value.Date);
        }

        if (endTime.HasValue)
        {
            query = query.Where(e => e.DateOfAction <= endTime.Value.Date);
        }

        return Data(MapperService.ViewModelMapper.Map<WorksheetItemModel[]>(query.OrderBy(e => e.DateOfAction).ToArray()));
    }

    [HttpGet]
    [Route("Projects")]
    public IActionResult GetProjects(int page, int pageSize, string search = null, bool showHidden = false)
    {
        using var db = EntitiesFactory.CreateDbContext();

        var projectsInOrganisations = ProjectsInOrganisation.Query(db, User.GetUserId())
                    .Where(e => (e.IdOrganisation == null && e.IdCreator == User.GetUserId())
                                || (e.IdOrganisation != null && e.IdAppUser == User.GetUserId()));

        if (!string.IsNullOrWhiteSpace(search))
        {
            return Data(MapperService.ViewModelMapper.Map<PageResultSet<GetProjectModel>>(projectsInOrganisations
            .Where(e => e.Name.Contains(search)).OrderBy(e => e.UserOrderNo).ForPagedResult(page, pageSize)));
        }

        return Data(MapperService.ViewModelMapper.Map<PageResultSet<GetProjectModel>>(projectsInOrganisations
                                                                                      .OrderBy(e => e.UserOrderNo)
                                                                                      .ForPagedResult(page, pageSize)));
    }

    [NonAction]
    private ProjectsInOrganisation[] ListProjects(bool? showHidden)
    {
        using var db = EntitiesFactory.CreateDbContext();
        var userId = User.GetUserId();

        if (showHidden.HasValue)
        {
            return ProjectsInOrganisation.Query(db, User.GetUserId())
                    .Where(e => (e.IdOrganisation == null && e.IdCreator == User.GetUserId())
                                || (e.IdOrganisation != null && e.IdAppUser == User.GetUserId()))
                                .Where(e => e.Hidden == showHidden).ToArray();
        }

        return ProjectsInOrganisation.Query(db, User.GetUserId())
                    .Where(e => (e.IdOrganisation == null && e.IdCreator == User.GetUserId())
                                || (e.IdOrganisation != null && e.IdAppUser == User.GetUserId())).ToArray();
    }

    [HttpGet]
    [Route("Overview")]
    public IActionResult GetOverview()
    {
        using var db = EntitiesFactory.CreateDbContext();
        var query = db.WorksheetItems
                        .Where(e => e.IdCreator == User.GetUserId())
                        .GroupBy(e =>
                        new
                        {
                            ItemRate = e.IdProjectItemRate,
                            ProjectId = e.IdWorksheetNavigation.IdProject,
                        })
                        .Join(db.Projects, e => e.Key.ProjectId, e => e.ProjectId, (group, proj) => new { group, proj })
                        .Join(db.ProjectItemRates, e => e.group.Key.ItemRate, e => e.ProjectItemRateId, (group, rate) => new { group = group.group, rate = rate, proj = group.proj })
                        .Select(e => new ProjectOverviewReporting()
                        {
                            IdCreator = e.proj.IdCreator,
                            ProjectName = e.proj.Name,
                            WorkedHours = e.group.Sum(f => f.ToTime - f.FromTime) / 60,
                            Earned = e.group.Sum(f => f.ToTime - f.FromTime) / 60 * e.rate.Rate,
                            Honorar = e.rate.Rate,
                            UserOrderNo = e.proj.UserOrderNo
                        })
                        .OrderBy(f => f.UserOrderNo)
                        .ToArray();

        return Data(MapperService.ViewModelMapper.Map<OverviewModel[]>(query));
    }

    [HttpGet]
    [Route("GetIneditableProjects")]
    public IActionResult NotEditableProjects()
    {
        using var db = EntitiesFactory.CreateDbContext();
        var map = db.SubmittedProjects
            .Where(e => e.IdCreator == User.GetUserId())
            .Select(f => new SubmittedProject()
            {
                IdCreator = f.IdCreator,
                ProjectId = f.ProjectId,
                Wcount = 1
            })
            .ToArray();

        return Data(MapperService.ViewModelMapper.Map<SubmittedProjectsModel[]>(map));
    }

    [HttpGet]
    [Route("GetByOrganisation")]
    public IActionResult GetProjectsByOrganisation(Guid organisationId)
    {
        using var db = EntitiesFactory.CreateDbContext();
        var userId = User.GetUserId();

        var isInOrg = db.OrganisationUserMaps.Any(e => e.IdOrganisation == organisationId && e.IdAppUser == userId);
        if (!isInOrg)
        {
            return Unauthorized("Org/NotPartOfOrg".AsTranslation());
        }

        var projects = db.Projects
            .Where(e => e.IdOrganisation == organisationId)
            .OrderBy(e => e.UserOrderNo)
            .ToArray();

        return Data(MapperService.ViewModelMapper.Map<GetProjectModel[]>(projects));
    }

    public override async ValueTask<IActionResult> Create(PostProjectApiModel model)
    {
        if (!await _userQuotaService.Add(User.GetUserId(), 1, Quotas.Project))
        {
            _logger.LogInformation("Exceeded Project Quota", LoggerCategories.Project.ToString());

            return BadRequest("You have Exceeded the max number of Projects");
        }

        if (!model.ChargeRates.Any())
        {
            return BadRequest("Common/NoChargeRate".AsTranslation());
        }

        var projectModel = model.Project;
        var db = EntitiesFactory.CreateDbContext();
        Project project = null;
        var addedRates = new List<ProjectItemRate>();

        using var transaction = await db.Database.BeginTransactionAsync().ConfigureAwait(false);

        if (projectModel.IdOrganisation.HasValue)
        {
            var org = db.Organisations.Find(projectModel.IdOrganisation.Value);

            if (!org.IsActive)
            {
                return BadRequest("The Organisation for this Project is Inactive.");
            }

            if (org.IsDeleted)
            {
                return BadRequest("The Organisation for this Project does no longer exists");
            }

            var roleInOrg = db.OrganisationUserMaps.FirstOrDefault(e => e.IdOrganisation == projectModel.IdOrganisation.Value && e.IdAppUser == User.GetUserId() && (
                e.IdRelation == UserToOrgRoles.ProjectManager.Id || e.IdRelation == UserToOrgRoles.Administrator.Id
            ));

            if (roleInOrg == null)
            {
                return BadRequest(
                                  "Only Project Manager of that Organisation can Create a Project for this Organisation");
            }
        }

        var fakeDefaultId = projectModel.IdDefaultRate;
        projectModel.IdDefaultRate = null;
        project = MapperService.ViewModelMapper.Map<Project>(projectModel);
        project.IdCreator = User.GetUserId();
        project.NumberRangeEntry = await _numberRangeService.GetNumberRangeAsync(db, ProjectNumberRangeFactory.NrCode, project.IdCreator, model);
        db.Add(project);
        await db.SaveChangesAsync().ConfigureAwait(false);

        ProjectItemRate defaultChargeRate = null;
        foreach (var modelChargeRate in model.ChargeRates)
        {
            var entity = MapperService.ViewModelMapper.Map<ProjectItemRate>(modelChargeRate.Entity);
            entity.IdCreator = User.GetUserId();
            entity.IdProjectNavigation = project;
            entity.IdProjectChargeRate = modelChargeRate.Entity.IdProjectChargeRate;
            entity.CurrencyType = modelChargeRate.Entity.CurrencyType;
            db.Add(entity);

            if (fakeDefaultId == entity.ProjectItemRateId)
            {
                defaultChargeRate = entity;
            }

            addedRates.Add(entity);
        }

        await db.SaveChangesAsync().ConfigureAwait(false);

        project.IdDefaultRate = defaultChargeRate.ProjectItemRateId!;

        await db.SaveChangesAsync().ConfigureAwait(false);

        await transaction.CommitAsync().ConfigureAwait(false);

        var externalProjectModel = MapperService.ViewModelMapper.Map<GetProjectModel>(project);
        _webHooks.PublishEvent(WebHookTypes.Project, WebHookTypes.Project.CreateHookObject(externalProjectModel, ActionTypes.Created), User.GetUserId(), Request.HttpContext.Connection.RemoteIpAddress.ToString());
        await _objectChangedService.SendObjectChanged(ChangeEventTypes.Added, project, Request.GetSignalId(), User.GetUserId());
        await _objectChangedService.SendObjectChanged(ChangeEventTypes.Added, addedRates.ToArray(), Request.GetSignalId(), User.GetUserId());

        return Data(externalProjectModel);

        // if (result is not OkResult)
        // {
        // 	return result;
        // }


    }

    public override async ValueTask<IActionResult> Update(PostProjectApiModel model, Guid id)
    {
        void UpdateChargeRates(MyworksheetContext db, Project? project1, PostProjectModel postProjectModel)
        {
            foreach (var modelChargeRate in model.ChargeRates)
            {
                if (modelChargeRate.Type == EntityListState.Deleted)
                {
                    db.ProjectItemRates.Where(e => e.ProjectItemRateId == modelChargeRate.Id).ExecuteDelete();
                }
                else
                {
                    var rateEntity =
                        MapperService.ViewModelMapper.Map<ProjectItemRate>(modelChargeRate.Entity);
                    rateEntity.IdProject = project1.ProjectId;
                    rateEntity.IdCreator = project1.IdCreator;

                    if (modelChargeRate.Type == EntityListState.Added)
                    {
                        db.Add(rateEntity);

                        if (rateEntity.ProjectItemRateId == postProjectModel?.IdDefaultRate)
                        {
                            postProjectModel.IdDefaultRate = rateEntity.IdProjectChargeRate;
                        }
                    }
                    else
                    {
                        db.Update(rateEntity);
                    }
                }
            }
        }

        var db = EntitiesFactory.CreateDbContext();

        var project = db.Projects.FirstOrDefault(e => e.IdCreator == User.GetUserId() && e.ProjectId == id);

        if (project == null || project.IdCreator != User.GetUserId())
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        if (project.Hidden)
        {
            return BadRequest("Project is deleted and cannot be changed");
        }

        var proj = model.Project;

        if (db.Worksheets.Any(f => f.IdCurrentStatus != Guid.Empty && f.IdProject == project.ProjectId))
        {
            if (proj != null)
            {
                db.Projects.Where(e => e.ProjectId == id)
                .ExecuteUpdate(f =>
                    f.SetProperty(e => e.UserOrderNo, proj.UserOrderNo)
                    .SetProperty(e => e.ProjectReference, proj.ProjectReference)
                    .SetProperty(e => e.IdPaymentCondition, proj.IdPaymentCondition)
                );
                project.UserOrderNo = proj.UserOrderNo;
                project.ProjectReference = proj.ProjectReference;
            }

            UpdateChargeRates(db, project, proj);

            db.SaveChanges();
            await _objectChangedService.SendObjectChanged(ChangeEventTypes.Changed, typeof(Project), id, Request.GetSignalId(), User.GetUserId());

            return Data(MapperService.ViewModelMapper.Map<GetProjectModel>(project));
        }

        using var transaction = db.Database.BeginTransaction();
        if (project.IdOrganisation.HasValue)
        {
            var org = db.Organisations.Find(project.IdOrganisation.Value);

            if (!org.IsActive)
            {
                return BadRequest("The Organisation for this Project is Inactive.");
            }

            if (org.IsDeleted)
            {
                return BadRequest("The Organisation for this Project does no longer exists");
            }

            var roleInOrg = db.OrganisationUserMaps.Where(e => e.IdOrganisation == project.IdOrganisation.Value && e.IdAppUser == User.GetUserId() && (
                e.IdRelation == UserToOrgRoles.ProjectManager.Id || e.IdRelation == UserToOrgRoles.Administrator.Id
            )).FirstOrDefault();

            if (roleInOrg == null)
            {
                return BadRequest("Only Project Manager of that Organisation can Update a Project for this Organisation");
            }
        }

        UpdateChargeRates(db, project, proj);

        if (proj != null)
        {
            MapperService.ViewModelMapper.Map(proj, project);
            db.Update(project);
        }

        db.SaveChanges();
        transaction.Commit();

        var externalProjectModel = MapperService.ViewModelMapper.Map<GetProjectModel>(project);

        _webHooks.PublishEvent(WebHookTypes.Project,
                               WebHookTypes.Project.CreateHookObject(externalProjectModel, ActionTypes.Updated),
                               User.GetUserId(),
                               Request.HttpContext.Connection.RemoteIpAddress.ToString());
        await _objectChangedService.SendObjectChanged(ChangeEventTypes.Changed, typeof(Project), externalProjectModel.ProjectId, Request.GetSignalId(), User.GetUserId());

        return Data(externalProjectModel);
    }

    [HttpPost]
    [Route("Delete")]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        _logger.LogInformation("Deleted Project", LoggerCategories.Project.ToString());
        var db = EntitiesFactory.CreateDbContext();

        var project = db.Projects.Where(e => e.IdCreator == User.GetUserId() && e.ProjectId == id).Include(e => e.Worksheets).FirstOrDefault();

        if (project == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var worksheets = project.Worksheets;
        var externalProjectModel = MapperService.ViewModelMapper.Map<GetProjectModel>(project);

        _webHooks.PublishEvent(WebHookTypes.Project,
                               WebHookTypes.Project.CreateHookObject(externalProjectModel, ActionTypes.Deleted),
                               User.GetUserId(),
                               Request.HttpContext.Connection.RemoteIpAddress.ToString());
        //AccessElement<ProjectHubInfo>.Instance.SendProjectChanged(project.ProjectId, externalProjectModel, ActionTypes.Deleted);

        if (!worksheets.Any(f =>
            {
                if (!f.IdWorksheetWorkflow.HasValue)
                {
                    return false;
                }

                var workflow = _worksheetWorkflowManager.WorksheetWorkflows[f.IdWorksheetWorkflow.Value];

                return !workflow.CanModify(_worksheetWorkflowManager.GetStepFromId(f.IdWorksheetWorkflow.Value, f.IdCurrentStatus));
            }))
        {
            var transaction = await db.Database.BeginTransactionAsync();
            await using (transaction.ConfigureAwait(false))
            {
                project.IdDefaultRate = null;

                foreach (var worksheet in worksheets)
                {
                    await WorksheetHelper.DeleteWorksheet(db,
                                                          worksheet.WorksheetId,
                                                          User.GetUserId(),
                                                          _budgetService,
                                                          _blobManagerService);
                }

                db.ProjectItemRates.Where(e => e.IdProject == id).ExecuteDelete();
                db.UserWorkloads.Where(e => e.IdProject == id).ExecuteDelete();
                db.Projects.Where(e => e.ProjectId == id).ExecuteDelete();

                await _userQuotaService.Subtract(User.GetUserId(), 1, Quotas.Project).ConfigureAwait(false);
                db.SaveChanges();
                transaction.Commit();
            }

            await _objectChangedService.SendObjectChanged(ChangeEventTypes.Removed, typeof(Project), externalProjectModel.ProjectId, Request.GetSignalId(), User.GetUserId());

            return Data();
        }

        project.Hidden = !project.Hidden;
        db.SaveChanges();

        await _objectChangedService.SendObjectChanged(ChangeEventTypes.Removed, typeof(Project), externalProjectModel.ProjectId, Request.GetSignalId(), User.GetUserId());

        return Data(externalProjectModel);
    }

    [HttpPost]
    [Route("ChangePosition")]
    public IActionResult ChangePosition(Guid projectid, int order)
    {
        using var db = EntitiesFactory.CreateDbContext();
        var projectByNo = db.Projects.FirstOrDefault(e => e.IdCreator == User.GetUserId() && e.ProjectId == projectid);
        if (projectByNo == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        db.Projects.Where(e => e.UserOrderNo > projectByNo.UserOrderNo && e.IdCreator == User.GetUserId())
            .ExecuteUpdate(e => e.SetProperty(f => f.UserOrderNo, w => w.UserOrderNo - 1));

        db.Projects.Where(e => e.UserOrderNo < projectByNo.UserOrderNo && e.IdCreator == User.GetUserId())
            .ExecuteUpdate(e => e.SetProperty(f => f.UserOrderNo, w => w.UserOrderNo + 1));

        db.Projects.Where(e => e.UserOrderNo < projectByNo.UserOrderNo && e.IdCreator == User.GetUserId() && e.ProjectId == projectid)
            .ExecuteUpdate(e => e.SetProperty(f => f.UserOrderNo, projectByNo.UserOrderNo));

        return Data();
    }
}
