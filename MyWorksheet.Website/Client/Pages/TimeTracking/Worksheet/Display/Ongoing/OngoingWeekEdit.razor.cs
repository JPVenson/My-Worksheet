using System;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.TimeTracking.Worksheet.Display.Ongoing;

public partial class OngoingWeekEdit
{
    [Inject]
    public HttpService HttpService { get; set; }

    [Parameter]
    public WorksheetWeekDisplay Week { get; set; }

    [Parameter]
    public WorksheetEditViewModel Model { get; set; }

    [Parameter]
    public DateTimeOffset SelectedDate { get; set; }

    private async Task UpdateWorksheetItem(WorksheetItemModel wsItem)
    {
        ServerErrorManager.Clear();
        using (WaiterService.WhenDisposed())
        {
            var apiResult = ServerErrorManager.Eval(await HttpService.WorksheetItemApiAccess.Update(wsItem, wsItem.WorksheetItemId));
            ServerErrorManager.DisplayStatus();
        }
        Render();
    }

    public async Task DeleteWorksheetItem(Guid id)
    {
        ServerErrorManager.Clear();
        using (WaiterService.WhenDisposed())
        {
            var apiResult = ServerErrorManager.Eval(await HttpService.WorksheetItemApiAccess.Delete(id));
            if (apiResult.Success)
            {
                Model.WorksheetItems.RemoveId(id);
            }
            ServerErrorManager.DisplayStatus();
        }
    }
}