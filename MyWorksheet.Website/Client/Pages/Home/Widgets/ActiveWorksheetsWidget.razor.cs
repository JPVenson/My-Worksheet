using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Home.Widgets;

public partial class ActiveWorksheetsWidget
{
    [Inject]
    public HttpService HttpService { get; set; }

    public DashboardWorksheetModel[] ActiveWorksheets { get; set; }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        ActiveWorksheets = ServerErrorManager.EvalAndUnbox(
            await HttpService.DashboardApiAccess.GetActiveWorksheets());
    }
}
