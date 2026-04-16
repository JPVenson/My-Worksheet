using System.Linq;
using MyWorksheet.Shared.Services.PriorityQueue;
using MyWorksheet.Shared.WebApi;
using MyWorksheet.Webpage.Helper.Roles;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Administration.ExecuteLater;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Controllers.Api.Administration;

[ApiController]
[RevokableAuthorize(Roles = Roles.AdminRoleName)]
[Route("api/ScheduledTaskRunnerApi")]
public class ScheduledTaskRunnerApiControllerBase : ApiControllerBase
{
    private readonly IMapperService _mapper;
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;
    private readonly IServerPriorityQueueManager _prioQueueMgr;

    public ScheduledTaskRunnerApiControllerBase(IMapperService mapper, IDbContextFactory<MyworksheetContext> dbContextFactory, IServerPriorityQueueManager prioQueueMgr)
    {
        _mapper = mapper;
        _dbContextFactory = dbContextFactory;
        _prioQueueMgr = prioQueueMgr;
    }

    [HttpGet]
    [Route("GetTasks")]
    public IActionResult GetTasks()
    {
        return Data(_mapper.ViewModelMapper.Map<PriorityQueueTaskViewModel[]>(_prioQueueMgr.PriorityQueueActions.Values));
    }

    [HttpGet]
    [Route("History")]
    public IActionResult GetHistory(string key)
    {
        using var db = _dbContextFactory.CreateDbContext();
        return Data(_mapper.ViewModelMapper.Map<PriorityQueueItemViewModel[]>(db.PriorityQueueItems.Where(e => e.ActionKey == key).ToArray()));
    }
}