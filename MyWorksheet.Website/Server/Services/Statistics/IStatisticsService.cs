using System.Collections.Generic;
using System;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Statistics;

namespace MyWorksheet.Website.Server.Services.Statistics;

public interface IStatisticsService
{
    Dictionary<StatisticsDataPresenter, IStatisticsProvider> StatisticsProviders { get; }

    IObjectSchema GetArgumentsSchema(string statisticsKey, MyworksheetContext db);
    DataExport GetData(string statisticsKey, Guid appUserId, IDictionary<string, object> argument,
        IEnumerable<Guid> projects, AggregationStrategy aggregateStrategy);
}