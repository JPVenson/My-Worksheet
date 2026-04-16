using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Workflow;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Shared.Services.Activation;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Client.Services.Reporting;

[SingletonService()]
public class ReportingService : LazyLoadedService
{
    private readonly HttpService _httpService;

    public ReportingService(HttpService httpService)
    {
        _httpService = httpService;
        ReportDataSources = new FutureList<ReportingDataSourceViewModel>(() => _httpService.ReportManagementApiAccess.Rds().AsTask());
        ReportDataSources.WhenLoaded(OnDataLoaded);
        Formatters = new FutureList<KeyValuePair<string, string>>(() => _httpService.ReportManagementApiAccess.GetTemplateFormatter().AsTask());
        Formatters.WhenLoaded(OnDataLoaded);
    }

    public IFutureList<ReportingDataSourceViewModel> ReportDataSources { get; set; }
    public IFutureList<KeyValuePair<string, string>> Formatters { get; set; }
}