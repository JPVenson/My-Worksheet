using System.Collections.Generic;
using System;
using System.Linq;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Statistics.Provider;
using MyWorksheet.Website.Shared.Services.Activation;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Statistics;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Services.Statistics;

[SingletonService(typeof(IStatisticsService))]
public class StatisticsService : IStatisticsService
{
    private readonly ActivatorService _activatorService;

    public StatisticsService(ActivatorService activatorService)
    {
        _activatorService = activatorService;
        StatisticsProviders = new Dictionary<StatisticsDataPresenter, IStatisticsProvider>
        {
            { new StatisticsDataPresenter("Overtime", "line"), activatorService.ActivateType<OvertimeStatisticsProvider>() },
            { new StatisticsDataPresenter("Worktime", "line"), activatorService.ActivateType<WorktimeStatisticsProvider>() },
            { new StatisticsDataPresenter("Actions", "bar"), activatorService.ActivateType<ActionCasesStatisticsProvider>() }
        };
    }

    public Dictionary<StatisticsDataPresenter, IStatisticsProvider> StatisticsProviders { get; private set; }

    public IObjectSchema GetArgumentsSchema(string statisticsKey, MyworksheetContext db)
    {
        return StatisticsProviders.FirstOrDefault(e => e.Key.Name.Equals(statisticsKey)).Value?.Arguments(db);
    }

    public DataExport GetData(string statisticsKey, Guid appUserId, IDictionary<string, object> argument, IEnumerable<Guid> projects, AggregationStrategy aggregateStrategy)
    {
        return StatisticsProviders.FirstOrDefault(e => e.Key.Name.Equals(statisticsKey)).Value?.GenerateSchema(appUserId, argument, projects, aggregateStrategy);
    }
}