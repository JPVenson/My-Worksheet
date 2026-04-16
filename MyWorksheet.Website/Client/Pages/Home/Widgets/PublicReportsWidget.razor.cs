using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Home.Widgets;

public partial class PublicReportsWidget
{
    [Inject]
    public HttpService HttpService { get; set; }

    public NEngineTemplateLookup[] PublicReports { get; set; }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        PublicReports = ServerErrorManager.EvalAndUnbox(
            await HttpService.DashboardApiAccess.GetDefaultReports());
    }
}
