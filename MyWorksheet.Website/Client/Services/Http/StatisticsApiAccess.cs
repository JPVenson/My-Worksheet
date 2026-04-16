using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Statistics;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Statistics;

namespace MyWorksheet.Website.Client.Services.Http;

public class StatisticsApiAccess : HttpAccessBase
{
    /// <inheritdoc />
    public StatisticsApiAccess(HttpService httpService) : base(httpService, "StatisticsApi")
    {

    }

    public ValueTask<ApiResult<StatisticsDataPresenterViewModel[]>> GetStatisticsProvider()
    {
        return Get<StatisticsDataPresenterViewModel[]>(BuildApi("GetStatisticsProvider"));
    }

    public ValueTask<ApiResult<AggregationStrategy>> GetAggregationStrategies()
    {
        return Get<AggregationStrategy>(BuildApi("GetAggregationStrategies"));
    }

    public ValueTask<ApiResult<IObjectSchema>> GetArgumentSchema(string statisticsKey)
    {
        return Get<IObjectSchema>(BuildApi("GetArgumentSchema", new { statisticsKey }));
    }

    public ValueTask<ApiResult<DataExportModel>> GetData(string statisticsKey,
        Guid[] projectIds,
        IDictionary<string, object> arguments,
        AggregationStrategy aggregation)
    {
        return Post<IDictionary<string, object>, DataExportModel>(BuildApi("GetData", new { statisticsKey, projectIds, aggregation }), arguments);
    }

}