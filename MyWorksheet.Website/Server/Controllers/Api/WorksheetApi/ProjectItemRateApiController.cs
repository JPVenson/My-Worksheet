using System.Linq;
using System;
using System.Threading.Tasks;
using MyWorksheet.Shared.WebApi;
using MyWorksheet.Webpage.Helper.Roles;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Services.ObjectChanged;
using MyWorksheet.Website.Server.Services.WorksheetWorkflowManagment;
using MyWorksheet.Website.Server.Util.Auth;
using MyWorksheet.Website.Server.Util.Extentions;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Controllers.Api.WorksheetApi;

[Route("api/ProjectItemRateApi")]
[RevokableAuthorize(Roles = Roles.AdminRoleName + "," + Roles.WorksheetAdminRoleName + "," + Roles.WorksheetUserRoleName)]
public class ProjectItemRateApiControllerBase : RestApiControllerBase<ProjectItemRate, ProjectItemRateViewModel>
{
    private readonly WorksheetWorkflowManager _worksheetWorkflowManager;
    private readonly ObjectChangedService _objectChangedService;

    public ProjectItemRateApiControllerBase(WorksheetWorkflowManager worksheetWorkflowManager,
        ObjectChangedService objectChangedService, IMapperService mapperService,
        IDbContextFactory<MyworksheetContext> dbContext) : base(dbContext, mapperService)
    {
        _worksheetWorkflowManager = worksheetWorkflowManager;
        _objectChangedService = objectChangedService;
    }

    [HttpGet]
    [Route("GetProjectRates")]
    public IActionResult GetRatesForProject(Guid projectId)
    {
        var db = EntitiesFactory.CreateDbContext();
        var rates = db.ProjectItemRates.Where(e => e.IdProject == projectId && e.IdCreator == User.GetUserId())
                        .ToArray();
        return Data(MapperService.ViewModelMapper.Map<ProjectItemRateViewModel[]>(rates));
    }

    [HttpGet]
    [Route("GetProjectsRates")]
    public IActionResult GetRatesForProjects([FromQuery] Guid[] projectIds)
    {
        var db = EntitiesFactory.CreateDbContext();
        return Data(MapperService.ViewModelMapper.Map<ProjectItemRateViewModel[]>(db.ProjectItemRates
        .Where(e => projectIds.Contains(e.IdProject) && e.IdCreator == User.GetUserId())
            .ToArray()));
    }

    [HttpGet]
    [Route("GetProjectChargeRates")]
    public IActionResult GetChargeRates()
    {
        var db = EntitiesFactory.CreateDbContext();
        return Data(MapperService.ViewModelMapper.Map<ProjectChargeRateModel[]>(db.ProjectChargeRates));
    }

    [HttpPost]
    [Route("Create")]
    public async Task<IActionResult> CreateRatesForProject([FromBody] ProjectItemRateViewModel itemRateCreate)
    {
        var db = EntitiesFactory.CreateDbContext();
        var entity = MapperService.ViewModelMapper.Map<ProjectItemRate>(itemRateCreate);
        entity.IdCreator = User.GetUserId();

        var project = db.Projects.Find(itemRateCreate.IdProject);
        if (project.IdCreator != entity.IdCreator)
        {
            return BadRequest();
        }
        db.Add(entity);
        db.SaveChanges();
        await _objectChangedService.SendObjectChanged(ChangeEventTypes.Added, entity, Request.GetSignalId());
        return Data(MapperService.ViewModelMapper.Map<ProjectItemRateViewModel>(entity));
    }

    [HttpPost]
    [Route("Delete")]
    public async Task<IActionResult> DeleteRatesForProject([FromQuery] Guid id)
    {
        var db = EntitiesFactory.CreateDbContext();
        var entity = db.ProjectItemRates.Where(e => e.ProjectItemRateId == id && e.IdCreator == User.GetUserId())
            .Include(e => e.WorksheetItems)
            .FirstOrDefault();

        if (entity == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        if (entity.WorksheetItems.Any())
        {
            return BadRequest("ProjectItemRate/Delete.RateInUse".AsTranslation());
        }

        db.ProjectItemRates.Remove(entity);
        db.SaveChanges();
        await _objectChangedService.SendObjectChanged(ChangeEventTypes.Removed, entity, Request.GetSignalId());
        return Data();
    }

    [HttpPost]
    [Route("Update")]
    public async Task<IActionResult> UpdateRatesForProject([FromBody] ProjectItemRateViewModel itemRateCreate)
    {
        var db = EntitiesFactory.CreateDbContext();
        var entity = db.ProjectItemRates.Where(e => e.ProjectItemRateId == itemRateCreate.ProjectItemRateId && e.IdCreator == User.GetUserId())
            .FirstOrDefault();

        if (entity == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var worksheets = db.Worksheets.Where(e => e.IdCreator == User.GetUserId() && e.IdProject == itemRateCreate.IdProject).ToArray();

        var submittedWorksheets = worksheets.Where(f =>
        {
            if (!f.IdWorksheetWorkflow.HasValue)
            {
                return false;
            }

            var workflow = _worksheetWorkflowManager.WorksheetWorkflows[f.IdWorksheetWorkflow.Value];
            return !workflow.CanModify(
                _worksheetWorkflowManager.GetStepFromId(f.IdWorksheetWorkflow.Value, f.IdCurrentStatus));
        }).ToArray();

        if (submittedWorksheets.Length != 0)
        {
            var usedIn = db.WorksheetItems.Where(e => submittedWorksheets.Select(e => e.WorksheetId).Contains(e.IdWorksheet) && e.IdProjectItemRate == itemRateCreate.ProjectItemRateId)
                .FirstOrDefault();

            if (usedIn != null)
            {
                return BadRequest("ProjectItemRate/Delete.RateInUse".AsTranslation());
            }
        }

        entity = MapperService.ViewModelMapper.Map(itemRateCreate, entity);
        db.Update(entity);
        db.SaveChanges();
        await _objectChangedService.SendObjectChanged(ChangeEventTypes.Changed, entity, Request.GetSignalId());
        return Data(MapperService.ViewModelMapper.Map<ProjectItemRateViewModel>(entity));
    }
}