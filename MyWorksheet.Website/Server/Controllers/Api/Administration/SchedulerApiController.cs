using System.Linq;
using MyWorksheet.Shared.WebApi;
using MyWorksheet.Webpage.Helper.Roles;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using MyWorksheet.Website.Server.Shared.TaskScheduling;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Administration.ExecuteLater;
using Microsoft.AspNetCore.Mvc;

namespace MyWorksheet.Website.Server.Controllers.Api.Administration;

[ApiController]
[Route("api/SchedulerApi")]
[RevokableAuthorize(Roles = Roles.AdminRoleName)]
public class SchedulerApiController : ApiControllerBase
{
    private readonly IMapperService _mapperService;
    private readonly ISchedulerService _schedulerService;
    private readonly IAppLogger _appLogger;

    public SchedulerApiController(IMapperService mapperService,
        ISchedulerService schedulerService,
        IAppLogger appLogger)
    {
        _mapperService = mapperService;
        _schedulerService = schedulerService;
        _appLogger = appLogger;
    }

    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    [Route("SchedulerInfos")]
    [HttpGet]
    public IActionResult GetSchedulerInfos()
    {
        return Ok(_mapperService.ViewModelMapper.Map<SchedulerInfo[]>(_schedulerService.Runners()));
    }

    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    [Route("Run")]
    [HttpPost]
    public IActionResult RunTask(string name)
    {
        var scheduledTask = _schedulerService.Runners().FirstOrDefault(e => e.Task.NamedTask == name);

        if (scheduledTask == null)
        {
            return BadRequest();
        }
        _schedulerService.RunOnceNow(scheduledTask.Task);
        return Ok();
    }
}