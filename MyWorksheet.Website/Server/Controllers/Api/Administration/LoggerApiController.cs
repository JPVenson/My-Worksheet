using System.Linq;
using MyWorksheet.Shared.WebApi;
using MyWorksheet.Webpage.Helper.Roles;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Controllers.Api.Administration;

[RevokableAuthorize(Roles = Roles.AdminRoleName)]
[Route("api/LoggerApi")]
[ApiController]
public class LoggerApiController : ApiControllerBase
{
    private readonly IAppLogger _logger;
    private readonly IDbContextFactory<MyworksheetContext> _contextFactory;
    private readonly IMapperService _mapperService;

    public LoggerApiController(IAppLogger logger,
        IDbContextFactory<MyworksheetContext> contextFactory,
        IMapperService mapperService)
    {
        _logger = logger;
        _contextFactory = contextFactory;
        _mapperService = mapperService;
    }

    [HttpGet]
    [Route("Get")]
    public IActionResult GetLatest(string key = null, int page = 1, int pageSize = 100)
    {
        using var db = _contextFactory.CreateDbContext();
        var query = db.AppLoggerLogs
        .OrderByDescending(e => e.DateInserted)
        .Skip((page - 1) * pageSize).Take(pageSize);
        //if (key != null)
        //{
        //	query = query.Column(f => f.Key).Is.StartWith(key);
        //}
        //.Column(f => f.Key).Is.StartWith(key);


        return Data(_mapperService.ViewModelMapper.Map<AppLoggerLogViewModel[]>(
            query.ToArray()));
    }
}