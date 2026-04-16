using System;
using System.Collections.Generic;
using MyWorksheet.Shared.WebApi;
using MyWorksheet.Webpage.Helper.Roles;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Services.Statistics;
using MyWorksheet.Website.Server.Util.Auth;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Statistics;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Statistics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Controllers.Api.WorksheetApi;

[RevokableAuthorize]
[Route("api/StatisticsApi")]
public class StatisticsApiControllerBase : ApiControllerBase
{
    private readonly IStatisticsService _statisticsService;

    public StatisticsApiControllerBase(IStatisticsService statisticsService, IDbContextFactory<MyworksheetContext> dbContextFactory, IMapperService mapper)
    {
        _statisticsService = statisticsService;
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
    }

    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;
    private readonly IMapperService _mapper;

    [RevokableAuthorize(Roles = Roles.WorksheetUserRoleName)]
    [HttpGet]
    [Route("GetStatisticsProvider")]
    public IActionResult GetStatistics()
    {
        return Data(_mapper.ViewModelMapper.Map<StatisticsDataPresenterViewModel[]>(_statisticsService.StatisticsProviders.Keys));
    }

    [RevokableAuthorize(Roles = Roles.WorksheetUserRoleName)]
    [HttpGet]
    [Route("GetArgumentSchema")]
    public IActionResult GetArgumentSchema(string statisticsKey)
    {
        using var db = _dbContextFactory.CreateDbContext();
        return Data(_statisticsService.GetArgumentsSchema(statisticsKey, db));
    }

    [RevokableAuthorize(Roles = Roles.WorksheetUserRoleName)]
    [HttpGet]
    [Route("GetAggregationStrategies")]
    public IActionResult GetAggregationStrategies()
    {
        return Data(Enum.GetNames(typeof(AggregationStrategy)));
    }

    [RevokableAuthorize(Roles = Roles.WorksheetUserRoleName)]
    [HttpPost]
    [Route("GetData")]
    public IActionResult GetData(string statisticsKey, Guid[] projectIds,
        [FromBody] IDictionary<string, object> arguments,
        AggregationStrategy aggregation = AggregationStrategy.AddWhereExistsAndDuplicateWhereNot)
    {
        //var projects = new List<int>();

        //var aggregationStrategy = default(AggregationStrategy);
        //if (arguments.ContainsKey("project_id"))
        //{
        //	projects.Add((int) arguments["project_id"]);
        //}
        //else if (arguments.ContainsKey("project_ids"))
        //{
        //	projects.AddRange((int[])arguments["project_ids"]);

        //	if (!arguments.ContainsKey("aggregationStrategy"))
        //	{
        //		return BadRequest("Missing aggregationStrategy Argument");
        //	}

        //	if (!Enum.TryParse((string) arguments["aggregationStrategy"], out aggregationStrategy))
        //	{
        //		return BadRequest("Invalid aggregationStrategy Argument");
        //	}
        //}
        //else
        //{
        //	return Unauthorized("Common/InvalidId".AsTranslation());
        //}

        return Data(_mapper.ViewModelMapper.Map<DataExportModel>(_statisticsService.GetData(statisticsKey, User.GetUserId(), arguments, projectIds, aggregation)));
    }
}