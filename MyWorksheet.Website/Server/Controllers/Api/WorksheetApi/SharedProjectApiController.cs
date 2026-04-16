using System;
using System.Linq;
using MyWorksheet.Shared.WebApi;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Services.Workflow;
using MyWorksheet.Website.Server.Util.Auth;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Controllers.Api.WorksheetApi;

[Route("Api/SharedProjectApiControllerBase")]
public class SharedProjectApiControllerBase : ApiControllerBase
{
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;
    private readonly IMapperService _mapper;

    public SharedProjectApiControllerBase(IDbContextFactory<MyworksheetContext> dbContextFactory, IMapperService mapper)
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
    }

    [HttpGet]
    [Route("GetSharedLink")]
    public IActionResult GetSharedLink(string code)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var sharedLink = db.ProjectShareKeys
            .Where(f => f.Key == code)
            .FirstOrDefault();
        if (sharedLink == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        return Data(_mapper.ViewModelMapper.Map<ProjectShareKeyModel>(sharedLink));
    }

    [HttpGet]
    [Route("GetSharedProjectContent")]
    public IActionResult GetProjectWithWorksheet(string code)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var sharedLink = db.ProjectShareKeys.Where(f => f.Key == code).FirstOrDefault();
        if (sharedLink == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        if (sharedLink.ExpiresAfter.HasValue && DateTime.Now > sharedLink.ExpiresAfter.Value)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var project = db.Projects.Find(sharedLink.IdProject);
        var worksheetsQuery = db.Worksheets
            .Where(f => f.IdProject == project.ProjectId)
            .Where(f => f.Hidden == false);
        if (!sharedLink.AllowNonSubmitted)
        {
            worksheetsQuery = worksheetsQuery.Where(f => f.IdCurrentStatus != WorksheetStatusType.Created.ConvertToGuid());
        }

        var worksheets = worksheetsQuery.ToArray();
        return Data(new GetProjectContentsModel()
        {
            Project = _mapper.ViewModelMapper.Map<GetProjectModel>(project),
            Worksheets = _mapper.ViewModelMapper.Map<WorksheetModel[]>(worksheets)
        });
    }

    [HttpGet]
    [Route("GetSharedProjectContent")]
    public IActionResult GetProjectWithWorksheet(string code, Guid worksheetId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var sharedLink = db.ProjectShareKeys.Where(f => f.Key == code).FirstOrDefault();
        if (sharedLink == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        if (sharedLink.ExpiresAfter.HasValue && DateTime.Now > sharedLink.ExpiresAfter.Value)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var project = db.Projects.Find(sharedLink.IdProject);
        var worksheetsQuery = db.Worksheets
            .Include(e => e.WorksheetItems)
            .Where(f => f.IdProject == project.ProjectId)
            .Where(f => f.Hidden == false)
            .Where(f => f.WorksheetId == worksheetId);
        if (!sharedLink.AllowNonSubmitted)
        {
            worksheetsQuery = worksheetsQuery.Where(f => f.IdCurrentStatus != WorksheetStatusType.Created.ConvertToGuid());
        }

        var worksheet = worksheetsQuery.FirstOrDefault();

        if (worksheet == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        return Data(new GetProjectDetailContentsModel()
        {
            Project = _mapper.ViewModelMapper.Map<GetProjectModel>(project),
            Worksheet = _mapper.ViewModelMapper.Map<WorksheetModel>(worksheet),
            WorksheetItems = _mapper.ViewModelMapper.Map<WorksheetItemModel[]>(worksheet.WorksheetItems)
        });
    }

    [HttpPost]
    [Route("CreateSharedLink")]
    [RevokableAuthorize]
    public IActionResult CreateSharedLink(ProjectShareKeyModel projectShareKeyModel)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var project = db.Projects.Find(projectShareKeyModel.IdProject);

        if (project.IdCreator != User.GetUserId())
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var sharedKey = _mapper.ViewModelMapper.Map<ProjectShareKey>(projectShareKeyModel);
        sharedKey.Key = Guid.NewGuid().ToString("N");
        db.Add(sharedKey);
        db.SaveChanges();
        return Data(_mapper.ViewModelMapper.Map<ProjectShareKeyModel>(sharedKey));
    }

    [HttpGet]
    [Route("GetSharedLinks")]
    [RevokableAuthorize]
    public IActionResult GetSharedLinks(Guid projectId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var project = db.Projects.Find(projectId);

        if (project.IdCreator != User.GetUserId())
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        return Data(_mapper.ViewModelMapper.Map<ProjectShareKeyModel[]>(db.ProjectShareKeys
            .Where(f => f.IdProject == projectId)
            .ToArray()));
    }

    [HttpPost]
    [Route("RevokeSharedKey")]
    [RevokableAuthorize]
    public IActionResult RevokeSharedKey(Guid projectSharedKeyId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var sharedKey = db.ProjectShareKeys
            .Include(e => e.IdProjectNavigation)
            .Where(e => e.ProjectShareKeyId == projectSharedKeyId)
            .FirstOrDefault();

        if (sharedKey == null || sharedKey.IdProjectNavigation.IdCreator != User.GetUserId())
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        db.ProjectShareKeys.Where(e => e.ProjectShareKeyId == projectSharedKeyId).ExecuteDelete();

        return Data();
    }

    [HttpPost]
    [Route("UpdateSharedLink")]
    [RevokableAuthorize]
    public IActionResult UpdateSharedLink(ProjectShareKeyModel projectShareKeyModel)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var sharedKey = db.ProjectShareKeys
            .Include(e => e.IdProjectNavigation)
            .Where(e => e.ProjectShareKeyId == projectShareKeyModel.ProjectShareKeyId)
            .FirstOrDefault();

        if (sharedKey == null || sharedKey.IdProjectNavigation.IdCreator != User.GetUserId())
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }
        sharedKey = this._mapper.ViewModelMapper.Map(projectShareKeyModel, sharedKey);
        db.Update(sharedKey);
        return Data();
    }
}