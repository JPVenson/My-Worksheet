using System;
using System.Collections.Generic;
using System.Linq;
using MyWorksheet.Shared.WebApi;
using MyWorksheet.Webpage.Helper.Roles;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using MyWorksheet.Website.Server.Util.Auth;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorksheetStatusLookupModel = MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.WorksheetStatusLookupModel;

namespace MyWorksheet.Website.Server.Controllers.Api.WorksheetApi;

[RevokableAuthorize(Roles = Roles.AdminRoleName + "," + Roles.WorksheetAdminRoleName + "," + Roles.WorksheetUserRoleName + "," + Roles.WorksheeActiontUserRoleName)]
[Route("api/WorksheetItemStatusApi")]
public class WorksheetItemStatusController : ApiControllerBase
{
    private readonly IMapperService _mapper;
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;
    private readonly IAppLogger _logger;

    public WorksheetItemStatusController(IMapperService mapper, IDbContextFactory<MyworksheetContext> dbContextFactory, IAppLogger logger)
    {
        _mapper = mapper;
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    [HttpGet]
    [Route("GetLookups")]
    public IActionResult GetLookups()
    {
        using var db = _dbContextFactory.CreateDbContext();
        return Data(_mapper.ViewModelMapper.Map<WorksheetStatusViewModel[]>(
            db.WorksheetItemStatusLookups
                .Where(f => f.IsHidden == false)
                .Where(f => f.IdAppUser == null || f.IdAppUser == User.GetUserId())
                .ToArray()));
    }

    [HttpPost]
    [Route("AlterLookup")]
    [RevokableAuthorize(Roles = Roles.AdminRoleName + "," + Roles.WorksheeActiontUserRoleName)]
    public IActionResult Alter(WorksheetStatusViewModel itemModel)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var item = db.WorksheetItemStatusLookups.Find(itemModel.WorksheetItemStatusLookupId);
        if (!User.IsInRole(Roles.AdminRoleName))
        {
            if (!item.IdAppUser.HasValue && !User.IsInRole(Roles.AdminRoleName))
            {
                _logger.LogCritical("Prevented attempted id injection occured! Method: AlterLookup.1", "Security", new Dictionary<string, string>()
                {
                    {
                        "UserId", User.GetUserId().ToString()
                    }
                });

                return BadRequest("Only admins can change a Public Action");
            }

            if (item.IdAppUser.HasValue && User.GetUserId() != item.IdAppUser.Value)
            {
                _logger.LogCritical("Prevented attempted id injection occured! Method: AlterLookup.2", "Security", new Dictionary<string, string>()
                {
                    {
                        "TestedId", item.IdAppUser.Value.ToString()
                    },
                    {
                        "UserId", User.GetUserId().ToString()
                    }
                });
                return BadRequest("Common/InvalidId".AsTranslation());
            }
        }
        db.Update(itemModel);
        db.SaveChanges();

        return Data(itemModel);
    }

    [HttpPost]
    [Route("CreateLookup")]
    [RevokableAuthorize(Roles = Roles.AdminRoleName + "," + Roles.WorksheeActiontUserRoleName)]
    public IActionResult Create(WorksheetItemStatusLookup item)
    {
        if (!User.IsInRole(Roles.AdminRoleName))
        {
            item.IdAppUser = User.GetUserId();
        }
        using var db = _dbContextFactory.CreateDbContext();
        db.Add(item);
        db.SaveChanges();
        return Data(_mapper.ViewModelMapper.Map<WorksheetStatusLookupModel>(item));
    }

    [HttpGet]
    [Route("GetStatusForWorksheet")]
    public IActionResult GetForWorksheet(Guid worksheetId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var worksheet = db.Worksheets.Where(e => e.IdCreator == User.GetUserId() && e.WorksheetId == worksheetId)
            .FirstOrDefault();

        if (worksheet == null || worksheet.IdCreator != User.GetUserId())
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        return Data(_mapper.ViewModelMapper.Map<WorksheetStatusLookupModel>(db.WorksheetItemStatuses.Where(e => e.IdWorksheet == worksheet.WorksheetId)));
    }

    [HttpGet]
    [Route("GetLookupsForWorksheet")]
    public IActionResult GetDirectForWorksheet(Guid worksheetId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var worksheet = db.Worksheets.Where(e => e.IdCreator == User.GetUserId() && e.WorksheetId == worksheetId)
            .FirstOrDefault();

        if (worksheet == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var lookups = db.WorksheetItemStatusLookups;
        var forWorksheet = db.WorksheetItemStatuses.Where(e => e.IdWorksheet == worksheet.WorksheetId).ToArray();

        return Data(forWorksheet.Select(e =>
        {
            var dayFound = lookups.FirstOrDefault(f => f.WorksheetItemStatusLookupId == e.IdWorksheetItemStatusLookup);
            if (dayFound == null)
            {
                return null;
            }

            var maped = _mapper.ViewModelMapper.Map<WorksheetStatusViewModel>(dayFound);
            maped.VirtualDay = e.DateOfAction;
            return maped;
        }));
    }

    [HttpPost]
    [Route("AddToWorksheet")]
    public IActionResult AddToWorksheet(Guid worksheetStatusId, Guid worksheetItem, DateTime dayOfMonth)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var worksheetItemStatus = db.WorksheetItemStatusLookups.Find(worksheetStatusId);
        if (worksheetItemStatus == null)
        {
            return BadRequest("The Status id does not Exist");
        }

        if (worksheetItemStatus.IdAppUser.HasValue && worksheetItemStatus.IdAppUser.Value != User.GetUserId())
        {
            _logger.LogCritical("Prevented attempted id injection occured! Method: AddToWorksheet.1", "Security", new Dictionary<string, string>()
            {
                {
                    "worksheetStatusId", worksheetStatusId.ToString()
                },
                {
                    "testedId", worksheetItemStatus.IdAppUser.Value.ToString()
                },
                {
                    "UserId", User.GetUserId().ToString()
                }
            });
        }

        var worksheet = db.Worksheets.Where(e => e.IdCreator == User.GetUserId() && e.WorksheetId == worksheetItem)
            .FirstOrDefault();

        if (worksheet == null)
        {
            return BadRequest("The Worksheet id does not Exist");
        }

        var hasStatus = db.WorksheetItemStatuses
            .Where(f => f.IdWorksheetItemStatusLookup == worksheetStatusId)
            .Where(f => f.IdWorksheet == worksheet.WorksheetId)
            .Where(f => f.DateOfAction == dayOfMonth)
            .FirstOrDefault();

        if (hasStatus != null)
        {
            return BadRequest("This status combination does allready exist");
        }
        var entity = new WorksheetItemStatus
        {
            IdWorksheetItemStatusLookup = worksheetStatusId,
            IdWorksheet = worksheet.WorksheetId,
            DateOfAction = dayOfMonth,
            IdCreator = User.GetUserId()
        };
        db.Add(entity);
        db.SaveChanges();
        return Data(entity);
    }

    [HttpPost]
    [Route("DeleteFromWorksheet")]
    public IActionResult DeleteFromWorksheet(Guid worksheetStatusId, Guid worksheetItem)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var worksheetItemStatus = db.WorksheetItemStatusLookups.Find(worksheetStatusId);
        if (worksheetItemStatus == null)
        {
            return BadRequest("The Status id does not Exist");
        }

        if (worksheetItemStatus.IdAppUser.HasValue && worksheetItemStatus.IdAppUser.Value != User.GetUserId())
        {
            _logger.LogCritical("Prevented attempted id injection occured! Method: AddToWorksheet.1", "Security", new Dictionary<string, string>()
            {
                {
                    "worksheetStatusId", worksheetStatusId.ToString()
                },
                {
                    "testedId", worksheetItemStatus.IdAppUser.Value.ToString()
                },
                {
                    "UserId", User.GetUserId().ToString()
                }
            });
        }

        var worksheet = db.Worksheets.Where(e => e.IdCreator == User.GetUserId() && e.WorksheetId == worksheetItem)
            .FirstOrDefault();

        if (worksheet == null)
        {
            return BadRequest("The Worksheet id does not Exist");
        }

        var hasStatus = db.WorksheetItemStatuses
            .Where(f => f.IdWorksheetItemStatusLookup == worksheetStatusId)
            .Where(f => f.IdWorksheet == worksheet.WorksheetId).FirstOrDefault();

        if (hasStatus == null)
        {
            return BadRequest("This status combination does not exist");
        }

        db.WorksheetItemStatuses.Where(e => e.WorksheetItemStatusId == hasStatus.WorksheetItemStatusId).ExecuteDelete();
        return Data();
    }
}