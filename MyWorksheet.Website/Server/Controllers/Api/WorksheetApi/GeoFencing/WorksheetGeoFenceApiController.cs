using System.Linq;
using MyWorksheet.Website.Shared.ViewModels;
using System;
using MyWorksheet.Helper.Db;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Util.Auth;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.GeoFencing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Controllers.Api.WorksheetApi.GeoFencing;

[Authorize]
[Route("api/WorksheetGeoFenceApi")]
public class WorksheetGeoFenceApiControllerBase : ApiControllerBase
{
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;
    private readonly IMapperService _mapper;

    public WorksheetGeoFenceApiControllerBase(IDbContextFactory<MyworksheetContext> dbContextFactory, IMapperService mapper)
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
    }

    [HttpGet]
    [Route("GetAll")]
    public IActionResult GetGeoFences()
    {
        using var db = _dbContextFactory.CreateDbContext();
        var userId = User.GetUserId();
        var fences = db.WorksheetGeoFences.Where(f => f.IdAppUser == userId).ToArray();
        return Data(_mapper.ViewModelMapper.Map<GetGeoFenceViewModel[]>(fences));
    }

    [HttpGet]
    [Route("GetGeoFenceDetail")]
    public IActionResult GetGeoFenceDetail(Guid geoFenceId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var userId = User.GetUserId();
        var geoFence = db.WorksheetGeoFences
            .Where(f => f.IdAppUser == userId && f.WorksheetGeoFenceId == geoFenceId)
            .FirstOrDefault();

        if (geoFence == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var fence = _mapper.ViewModelMapper.Map<GetGeoFenceViewModel>(geoFence);
        fence.GeoPositions = _mapper.ViewModelMapper.Map<GeoPositionViewModel[]>(
            db.WorksheetGeoFenceLocations.Where(f => f.IdWorksheetGeoFence == geoFenceId).ToArray());
        fence.GeoWiFis = _mapper.ViewModelMapper.Map<GeoWiFiNameViewModel[]>(
            db.WorksheetGeoFenceWiFis.Where(f => f.IdWorksheetGeoFence == geoFenceId).ToArray());

        return Data(fence);
    }

    [HttpPost]
    [Route("Delete")]
    public IActionResult DeleteGeoFence(Guid geoFenceId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var userId = User.GetUserId();
        var geoFence = db.WorksheetGeoFences.Find(geoFenceId);

        if (geoFence == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var project = db.Projects
            .Where(f => f.IdCreator == userId && f.ProjectId == geoFence.IdProject)
            .FirstOrDefault();
        if (project == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        db.WorksheetGeoFenceWiFis.Where(f => f.IdWorksheetGeoFence == geoFenceId).ExecuteDelete();
        db.WorksheetGeoFenceLocations.Where(f => f.IdWorksheetGeoFence == geoFenceId).ExecuteDelete();
        db.WorksheetGeoFences.Remove(geoFence);
        db.SaveChanges();

        return Data();
    }

    [HttpPost]
    [Route("Change")]
    public IActionResult ChangeGeoFence([FromBody] GetGeoFenceViewModel model)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var userId = User.GetUserId();
        var project = db.Projects
            .Where(f => f.IdCreator == userId && f.ProjectId == model.IdProject)
            .FirstOrDefault();
        if (project == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var fence = db.WorksheetGeoFences.Find(model.WorksheetGeoFenceId);
        if (fence == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        fence.Name = model.Name;
        fence.IsEnabled = model.IsEnabled;
        db.WorksheetGeoFences.Update(fence);

        var wiFiEndpoints = _mapper.ViewModelMapper.Map<WorksheetGeoFenceWiFi[]>(model.GeoWiFis);
        var positionsEndpoints = _mapper.ViewModelMapper.Map<WorksheetGeoFenceLocation[]>(model.GeoPositions);

        foreach (var wiFi in wiFiEndpoints)
        {
            wiFi.IdWorksheetGeoFence = fence.WorksheetGeoFenceId;
        }

        foreach (var pos in positionsEndpoints)
        {
            pos.IdWorksheetGeoFence = fence.WorksheetGeoFenceId;
        }

        var existingWiFis = db.WorksheetGeoFenceWiFis.Where(f => f.IdWorksheetGeoFence == fence.WorksheetGeoFenceId).ToArray();
        var existingPositions = db.WorksheetGeoFenceLocations.Where(f => f.IdWorksheetGeoFence == fence.WorksheetGeoFenceId).ToArray();

        db.DoCreateDeleteOrUpdate(existingWiFis, wiFiEndpoints, f => f.WorksheetGeoFenceWiFiId);
        db.DoCreateDeleteOrUpdate(existingPositions, positionsEndpoints, f => f.WorksheetGeoFenceId);

        db.SaveChanges();
        return Data();
    }

    [HttpPost]
    [Route("Create")]
    public IActionResult CreateGeoFence([FromBody] GetGeoFenceViewModel model)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var userId = User.GetUserId();
        var project = db.Projects
            .Where(f => f.IdCreator == userId && f.ProjectId == model.IdProject)
            .FirstOrDefault();
        if (project == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var worksheetGeoFence = new WorksheetGeoFence
        {
            IdProject = project.ProjectId,
            Name = model.Name,
            IsEnabled = false,
            IdAppUser = userId
        };
        db.WorksheetGeoFences.Add(worksheetGeoFence);
        db.SaveChanges();

        foreach (var wifi in model.GeoWiFis)
        {
            db.WorksheetGeoFenceWiFis.Add(new WorksheetGeoFenceWiFi
            {
                Name = wifi.Name,
                IdWorksheetGeoFence = worksheetGeoFence.WorksheetGeoFenceId
            });
        }

        foreach (var geoPoint in model.GeoPositions)
        {
            db.WorksheetGeoFenceLocations.Add(new WorksheetGeoFenceLocation
            {
                Latitude = geoPoint.Latitude,
                Longitude = geoPoint.Longitude,
                IdWorksheetGeoFence = worksheetGeoFence.WorksheetGeoFenceId
            });
        }

        db.SaveChanges();
        return Data(worksheetGeoFence.WorksheetGeoFenceId);
    }
}