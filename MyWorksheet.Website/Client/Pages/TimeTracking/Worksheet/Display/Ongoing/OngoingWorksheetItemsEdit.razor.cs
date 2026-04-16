using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Collapse;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.UserWorkload;
using MyWorksheet.Website.Client.Shared.Layout;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.TimeTracking.Worksheet.Display.Ongoing;

public partial class OngoingWorksheetItemsEdit
{
    private DateTimeOffset _selectedDate;

    [Parameter]
    public WorksheetEditViewModel Model { get; set; }
    [Inject]
    public UserWorkloadService UserWorkloadService { get; set; }
    [Inject]
    public HttpService HttpService { get; set; }

    public DateTimeOffset SelectedDate
    {
        get { return _selectedDate; }
        set
        {
            _selectedDate = value;
            ReevaluateEditorFields();
        }
    }

    private void ReevaluateEditorFields()
    {
        Editor.DateOfAction = SelectedDate;
        var hasWsInDate = Model
            .WorksheetItems
            .Where(e => e.DateOfAction.Date == SelectedDate.Date)
            .ToArray();
        if (hasWsInDate.Any())
        {
            Editor.FromTime = hasWsInDate.Select(f => f.ToTime).Max();
            Editor.ToTime = Math.Min(Editor.FromTime + 120, 60 * 24);
        }
        else
        {
            var now = DateTimeOffset.Now;
            if (SelectedDate.Date == now.Date)
            {
                Editor.FromTime = now.Hour * 60 + now.Minute;
                Editor.ToTime = Math.Min(Editor.FromTime + 120, 60 * 24);
            }
        }
    }

    public WorksheetItemEditor Editor { get; set; }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        Editor = new WorksheetItemEditor()
        {
            IdProjectItemRate = Model.ChargeRates.FirstOrDefault()?.ProjectItemRateId
        };
        SelectedDate = DateTimeOffset.Now;
        AddDisposable(LayoutController.Modifier(f =>
        {
            f.FullHeightContent = true;
        }));
    }

    private async Task CreateNewWorksheetItem(WorksheetItemEditor editor)
    {
        if (!CanSaveEditorItem(editor))
        {
            return;
        }
        ServerErrorManager.Clear();
        using (WaiterService.WhenDisposed())
        {
            var apiResult = ServerErrorManager.Eval(await HttpService.WorksheetItemApiAccess.Create(new WorksheetItemModel()
            {
                DateOfAction = editor.DateOfAction,
                ToTime = editor.ToTime,
                FromTime = editor.FromTime,
                Comment = editor.Comment,
                IdProjectItemRate = editor.IdProjectItemRate ?? Model.Project.IdDefaultRate.Value,
                IdWorksheet = Model.Worksheet.WorksheetId,
            }));
            if (apiResult.Success)
            {
                Model.WorksheetItems.Add(apiResult.Object);
                editor.FromTime = apiResult.Object.ToTime;
                editor.ToTime = editor.FromTime + 120;
                await Model.RebuildWorksheetItems();
            }
            ServerErrorManager.DisplayStatus();
        }
        Render();
    }

    private bool CanSaveEditorItem(WorksheetItemEditor editor)
    {
        if (editor.FromTime == editor.ToTime)
        {
            return false;
        }
        if (editor.ToTime < editor.FromTime)
        {
            return false;
        }
        if (editor.ToTime > 1440)
        {
            return false;
        }

        return true;
    }
}