using System.Collections.Generic;
using System;
using System.Linq;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Shared.ObjectSchema;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Statistics;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Services.Statistics.Provider;

public abstract class StatisticsProviderBase : IStatisticsProvider
{
    protected IDbContextFactory<MyworksheetContext> DbContextFactory { get; }

    public StatisticsProviderBase(IDbContextFactory<MyworksheetContext> dbContextFactory)
    {
        DbContextFactory = dbContextFactory;
    }

    public abstract string Display { get; }
    public abstract IObjectSchema Arguments(MyworksheetContext db);

    public abstract DataExport DataExport(Guid appUserId, IDictionary<string, object> arguments, Guid projects);

    public DataExport GenerateSchema(Guid appUserId, IDictionary<string, object> arg, IEnumerable<Guid> projects, AggregationStrategy aggregateStrategy)
    {
        var db = DbContextFactory.CreateDbContext();

        var schema = Arguments(db);
        var validationErrors = schema.Validate(arg);

        if (validationErrors.Any())
        {
            return null;
        }

        var dataExports = projects.Select(e => DataExport(appUserId, arg, e)).ToArray();

        if (dataExports.Any())
        {
            return dataExports.Aggregate((e, f) => e.Aggregate(f, aggregateStrategy));
        }

        return new DataExport();
    }
}