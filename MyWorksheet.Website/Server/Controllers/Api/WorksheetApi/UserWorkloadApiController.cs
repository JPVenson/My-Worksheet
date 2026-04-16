using System.Linq;
using System;
using System.Threading.Tasks;
using MyWorksheet.Shared.WebApi;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Services.ObjectChanged;
using MyWorksheet.Website.Server.Services.Worktime;
using MyWorksheet.Website.Server.Util.Auth;
using MyWorksheet.Website.Server.Util.Extentions;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.UserWorkload;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Controllers.Api.WorksheetApi;

[RevokableAuthorize]
[Route("api/UserWorkloadApi")]
public class UserWorkloadControllerBase : RestApiControllerBase<UserWorkload, GetUserWorkloadViewModel>
{
    private readonly IUserWorktimeService _userWorktimeService;
    private readonly ObjectChangedService _objectChangedService;

    public UserWorkloadControllerBase(IMapperService mapper,
        IDbContextFactory<MyworksheetContext> dbContextFactory,
        IUserWorktimeService userWorktimeService,
        ObjectChangedService objectChangedService) : base(dbContextFactory, mapper)
    {
        _userWorktimeService = userWorktimeService;
        _objectChangedService = objectChangedService;
    }

    [HttpGet]
    [Route("GetWorkloadForProject")]
    public IActionResult GetWorkloadForProject(Guid projectId)
    {
        using var db = EntitiesFactory.CreateDbContext();
        var workload = _userWorktimeService.GetWorkloadForProject(db, projectId, User.GetUserId());
        return Data(MapperService.ViewModelMapper.Map<GetUserWorkloadViewModel>(workload));
    }

    [HttpGet]
    [Route("GetWorkloadsForProjects")]
    public IActionResult GetWorkloadsForProjects([FromQuery] Guid[] projectIds)
    {
        if (!projectIds.Any())
        {
            return NoContent();
        }

        using var db = EntitiesFactory.CreateDbContext();
        var workload = _userWorktimeService.GetWorkloadsForProject(db, projectIds.Distinct().ToArray(), User.GetUserId());
        return Data(MapperService.ViewModelMapper.Map<GetUserWorkloadViewModel[]>(workload));
    }

    [HttpGet]
    [Route("GetUsersWorkload")]
    public IActionResult GetUsersWorkload()
    {
        using var db = EntitiesFactory.CreateDbContext();
        var workload = db.UserWorkloads
            .Where(f => f.IdProject == null)
            .Where(f => f.IdAppUser == User.GetUserId())
            .FirstOrDefault();

        return Data(MapperService.ViewModelMapper.Map<GetUserWorkloadViewModel>(workload));
    }

    protected override UserWorkload[] GetAllByUser()
    {
        using var db = EntitiesFactory.CreateDbContext();
        return EntitiesFactory.CreateDbContext().UserWorkloads
            .Where(f => f.IdAppUser == User.GetUserId())
            .ToArray();
    }

    [HttpPost]
    [Route("Update")]
    public async Task<IActionResult> UpdateWorkload([FromBody] UpdateUserWorkloadViewModel model)
    {
        var db = EntitiesFactory.CreateDbContext();
        var workload = db.UserWorkloads
            .Where(e => e.UserWorkloadId == model.UserWorkloadId)
            .Where(f => f.IdAppUser == User.GetUserId())
            .FirstOrDefault();

        if (workload == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        model.UserWorkloadId = workload.UserWorkloadId;
        workload = MapperService.ViewModelMapper.Map(model, workload);
        db.Update(workload);
        await _objectChangedService.SendObjectChanged(ChangeEventTypes.Changed, workload, Request.GetSignalId(), User.GetUserId());
        return Data(MapperService.ViewModelMapper.Map<GetUserWorkloadViewModel>(workload));
    }

    [HttpPost]
    [Route("Delete")]
    public async Task<IActionResult> DeleteWorkload(Guid userWorkloadId)
    {
        var db = EntitiesFactory.CreateDbContext();
        var workload = db.UserWorkloads
            .Where(e => e.UserWorkloadId == userWorkloadId)
            .Where(f => f.IdAppUser == User.GetUserId())
            .FirstOrDefault();

        if (workload == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        if (!workload.IdProject.HasValue && !workload.IdOrganisation.HasValue)
        {
            return BadRequest("Deleting the Default Workload is not supported");
        }

        db.UserWorkloads.Remove(workload);
        db.SaveChanges();
        await _objectChangedService.SendObjectChanged(ChangeEventTypes.Removed, workload, Request.GetSignalId(), User.GetUserId());
        return Data();
    }

    [HttpPost]
    [Route("Create")]
    public async Task<IActionResult> CreateWorkload([FromBody] CreateUserWorkloadViewModel model)
    {
        var db = EntitiesFactory.CreateDbContext();
        var workload = db.UserWorkloads
            .Where(f => f.IdAppUser == User.GetUserId())
            .Where(f => f.IdProject == model.IdProject)
            .Where(f => f.IdOrganisation == model.IdOrganisation)
            .FirstOrDefault();

        if (workload != null)
        {
            return BadRequest("UserWorkload/Errors.KindExists".AsTranslation());
        }

        workload = MapperService.ViewModelMapper.Map<UserWorkload>(model);
        workload.IdAppUser = User.GetUserId();

        db.Add(workload);
        db.SaveChanges();
        await _objectChangedService.SendObjectChanged(ChangeEventTypes.Added, workload, Request.GetSignalId(), User.GetUserId());
        return Data(MapperService.ViewModelMapper.Map<GetUserWorkloadViewModel>(workload));
    }
}