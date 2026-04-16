using System.Collections.Generic;
using System;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Statistics;

namespace MyWorksheet.Website.Server.Services.Statistics;

public interface IStatisticsProvider
{
    string Display { get; }
    IObjectSchema Arguments(MyworksheetContext db);
    DataExport GenerateSchema(Guid appUserId, IDictionary<string, object> arg, IEnumerable<Guid> projects,
        AggregationStrategy aggregateStrategy);
}