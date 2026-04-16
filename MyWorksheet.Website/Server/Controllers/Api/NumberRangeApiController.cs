using System;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Services.NumberRangeService;
using MyWorksheet.Website.Server.Util.Auth;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.NumberRange;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Controllers.Api;

[Route("api/NumberRangeApi")]
[Authorize]
public class NumberRangeApiController : ApiControllerBase
{
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;
    private readonly IMapperService _mapperService;
    private readonly INumberRangeService _numberRangeService;

    public NumberRangeApiController(IDbContextFactory<MyworksheetContext> dbContextFactory, IMapperService mapperService, INumberRangeService numberRangeService)
    {
        _dbContextFactory = dbContextFactory;
        _mapperService = mapperService;
        _numberRangeService = numberRangeService;
    }

    [HttpGet]
    [Route("Get")]
    public IActionResult GetNumberRanges()
    {
        using var db = _dbContextFactory.CreateDbContext();
        return Data(_mapperService.ViewModelMapper.Map<NumberRangeModel[]>(db.AppNumberRanges
            .Where(f => f.IdUser == User.GetUserId())
            .Where(f => f.IsActive == true)
            .OrderBy(e => e.Code)));
    }

    [HttpGet]
    [Route("GetSingle")]
    public IActionResult GetNumberRange(Guid id)
    {
        using var db = _dbContextFactory.CreateDbContext();
        return Data(_mapperService.ViewModelMapper.Map<NumberRangeModel>(db.AppNumberRanges
            .Where(f => f.IdUser == User.GetUserId())
            .Where(f => f.IsActive == true)
            .Where(e => e.AppNumberRangeId == id).FirstOrDefault()));
    }

    [HttpGet]
    [Route("GetCodeMappings")]
    public IActionResult GetCodeMappings()
    {
        return Data(_numberRangeService.NumberRangeFactories.Values.Select(f => new NumberRangeCodeMap()
        {
            Code = f.Code,
            DescriptionText = f.Description
        }));
    }

    [HttpGet]
    [Route("JsonSchema")]
    public IActionResult GetNumberRangeStructure(string code)
    {
        using var db = _dbContextFactory.CreateDbContext();
        return Data(_numberRangeService.NumberRangeFactories[code].GetSchema(db));
    }

    [HttpGet]
    [Route("Test")]
    public async Task<IActionResult> TestTemplate(string code, string template, long counter)
    {
        return Data(await _numberRangeService.Test(code, template, counter));
    }

    [HttpPost]
    [Route("Update")]
    public IActionResult Update([FromBody] CreateNumberRangeModel model)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var entity = _mapperService.ViewModelMapper.Map<AppNumberRange>(model);
        entity.IdUser = User.GetUserId();
        var transaction = db.Database.BeginTransaction();

        db.AppNumberRanges
                        .Where(f => f.Code == entity.Code)
                        .Where(f => f.IdUser == entity.IdUser)
                        .ExecuteUpdate(e => e.SetProperty(f => f.IsActive, false));

        db.Add(entity);

        db.SaveChanges();
        transaction.Commit();

        return Data(_mapperService.ViewModelMapper.Map<NumberRangeModel>(entity));
    }
}