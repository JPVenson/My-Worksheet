using System;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Dashboard;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

namespace MyWorksheet.Website.Client.Services.Http;

public class DashboardApiAccess : HttpAccessBase
{
    public DashboardApiAccess(HttpService httpService) : base(httpService, "DashboardApi")
    {
    }

    public ValueTask<ApiResult<DashboardPluginInfoViewModel[]>> GetDashboardPluginInfos()
        => Get<DashboardPluginInfoViewModel[]>(BuildApi("DashboardPluginInfos"));

    public ValueTask<ApiResult> UpdateDashboardPluginInfos(DashboardPluginInfoViewModel[] plugins)
        => Post(BuildApi("UpdateDashboardPluginInfos"), plugins);

    public ValueTask<ApiResult<DashboardWorksheetModel[]>> GetActiveWorksheets()
        => Get<DashboardWorksheetModel[]>(BuildApi("ActiveWorksheets"));

    public ValueTask<ApiResult<InvoiceViewModel[]>> GetInvoicesFor(DateTimeOffset startDate, DateTimeOffset endDate)
        => Get<InvoiceViewModel[]>(BuildApi("GetInvoicesFor", new { startDate, endDate }));

    public ValueTask<ApiResult<NEngineTemplateLookup[]>> GetDefaultReports()
        => Get<NEngineTemplateLookup[]>(BuildApi("DefaultReports"));
}
