using System;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Dashboard;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Home.Widgets;

public partial class EarningsWidget
{
    [Inject]
    public HttpService HttpService { get; set; }

    public InvoiceViewModel[] Invoices { get; set; }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        var end = DateTimeOffset.Now;
        var start = end.AddDays(-14);
        Invoices = ServerErrorManager.EvalAndUnbox(
            await HttpService.DashboardApiAccess.GetInvoicesFor(start, end));
    }
}
