using System.Collections.Generic;
using System;
using System.Linq;
using MyWorksheet.Shared.WebApi;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using MyWorksheet.Website.Server.Util.Auth;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Workflow;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Controllers.Api.WorksheetApi;

[Route("api/WorksheetHistoryApi")]
[RevokableAuthorize]
public class WorksheetHistoryApiControllerBase : ApiControllerBase
{
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;
    private readonly IAppLogger _logger;
    private readonly IMapperService _mapper;

    public WorksheetHistoryApiControllerBase(IDbContextFactory<MyworksheetContext> dbContextFactory, IAppLogger logger, IMapperService mapper)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet]
    [RevokableAuthorize]
    [Route("Lookups")]
    public IActionResult GetLookups()
    {
        using var db = _dbContextFactory.CreateDbContext();
        return Data(_mapper.ViewModelMapper.Map<WorksheetWorkflowStatusLookupViewModel[]>(db.WorksheetStatusLookups));
    }

    [HttpGet]
    [RevokableAuthorize]
    [Route("WorksheetHistory/Latest")]
    public IActionResult GetLatestStatus(Guid worksheetId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var worksheet = db.Worksheets
            .Where(e => e.IdCreator == User.GetUserId() && e.WorksheetId == worksheetId)
            .FirstOrDefault();
        if (worksheet == null)
        {
            _logger.LogCritical("Prevented attempted id injection occured! Method: GetStatus.1", "Security",
                new Dictionary<string, string>()
                {
                    {
                        "virtualId", worksheetId.ToString()
                    },
                    {
                        "UserId", User.GetUserId().ToString()
                    }
                });
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var worksheetStatusHistory = db.WorksheetStatusHistories.Where(f => f.IdWorksheet == worksheet.WorksheetId).OrderByDescending(f => f.DateOfAction).FirstOrDefault();

        return Data(_mapper.ViewModelMapper.Map<WorksheetStatusModel>(worksheetStatusHistory));
    }

    [HttpGet]
    [RevokableAuthorize]
    [Route("WorksheetHistory")]
    public IActionResult GetHistory(Guid worksheetId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var worksheet = db.Worksheets.Where(e => e.IdCreator == User.GetUserId() && e.WorksheetId == worksheetId)
            .FirstOrDefault();
        if (worksheet == null)
        {
            _logger.LogCritical("Prevented attempted id injection occured! Method: GetStatus.1", "Security",
                new Dictionary<string, string>()
                {
                    {
                        "virtualId", worksheetId.ToString()
                    },
                    {
                        "UserId", User.GetUserId().ToString()
                    }
                });
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var worksheetStatusHistory = db.WorksheetStatusHistories.Where(f => f.IdWorksheet == worksheet.WorksheetId).OrderByDescending(f => f.DateOfAction).ToArray();

        return Data(_mapper.ViewModelMapper.Map<WorksheetStatusModel[]>(worksheetStatusHistory));
    }
}