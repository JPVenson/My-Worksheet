using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Presentation;
using MyWorksheet.Website.Client.Services.WaiterIndicator;
using MyWorksheet.Website.Client.Util;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.TimeTracking.Worksheet.Display.PerWeek;

public partial class WorksheetItemEditDayComponent
{
    public WorksheetItemEditDayComponent()
    {
        Editors = new Dictionary<Guid, KeyValuePair<WorksheetItemModel, EntityState<WorksheetItemEditorBatchEdit>>>();
    }

    [Parameter]
    public WorksheetDayDisplay WorksheetDay { get; set; }
    [Parameter]
    public WorksheetEditViewModel Model { get; set; }
    [Parameter]
    public WorksheetTimeTrackerViewModel Tracker { get; set; }
    [Parameter]
    public IEnumerable<ProjectItemRateViewModel> ItemRates { get; set; }

    [Inject]
    public HttpService HttpService { get; set; }
    [Inject]
    public PresentationModeService PresentationModeService { get; set; }

    public IDictionary<Guid, KeyValuePair<WorksheetItemModel, EntityState<WorksheetItemEditorBatchEdit>>> Editors { get; set; }

    private WorksheetItemEditorBatchEdit StartEditing(WorksheetItemModel wsItemModel)
    {
        if (Editors.TryGetValue(wsItemModel.WorksheetItemId, out var existing))
        {
            return existing.Value.Entity;
        }

        var worksheetItemEditorBatchEdit = new WorksheetItemEditorBatchEdit()
        {
            Comment = wsItemModel.Comment,
            DateOfAction = wsItemModel.DateOfAction,
            FromTime = wsItemModel.FromTime,
            IdProjectItemRate = wsItemModel.IdProjectItemRate,
            ToTime = wsItemModel.ToTime
        };
        Editors[wsItemModel.WorksheetItemId] = new KeyValuePair<WorksheetItemModel, EntityState<WorksheetItemEditorBatchEdit>>(wsItemModel, worksheetItemEditorBatchEdit);
        return worksheetItemEditorBatchEdit;
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
                await InvokeAsync(() =>
                {
                    if (Model.WorksheetItems.All(f => f.WorksheetItemId != apiResult.Object.WorksheetItemId))
                    {
                        Model.WorksheetItems.Add(apiResult.Object);
                    }

                    editor.FromTime = apiResult.Object.ToTime;
                    editor.ToTime = Math.Max(editor.FromTime + 120, 60 * 24);
                    editor.Comment = "";
                });
                ServerErrorManager.DisplayStatus();
            }
            else if (apiResult.StatusCode == HttpStatusCode.Conflict)
            {
                WaiterService.DisplayBadge(new BadgeDisplay()
                {
                    Icon = "fas fa-exclamation-circle fa-2x text-warning",
                    Text = "WorksheetItem/Overlapping"
                });
            }
            else
            {
                ServerErrorManager.DisplayStatus();
            }

        }
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

        if (Tracker != null && Tracker.StartTime >= editor.DateOfAction)
        {
            return false;
        }

        return true;
    }

    public async Task SaveMultiple(DateTime dayDate)
    {
        ServerErrorManager.Clear();
        var modifiedItems = Editors.Where(e => e.Value.Value.Entity.DateOfAction.Date == dayDate).ToArray();
        CheckOverlaps(modifiedItems.Select(f => new KeyValuePair<Guid, WorksheetItemEditorBatchEdit>(f.Key, f.Value.Value.Entity)));
        if (Editors.Any(f => f.Value.Value.Entity.Overlaps))
        {
            StateHasChanged();
            WaiterService.DisplayBadge(new BadgeDisplay()
            {
                Icon = "fas fa-exclamation-circle fa-2x text-warning",
                Text = "WorksheetItem/Overlapping"
            });
            return;
        }

        using (WaiterService.WhenDisposed())
        {
            foreach (var keyValuePair in modifiedItems.Where(e => e.Value.Value.IsObjectDirty))
            {
                var apiResult = ServerErrorManager.Eval(await HttpService.WorksheetItemApiAccess.Update(new WorksheetItemModel()
                {
                    Comment = keyValuePair.Value.Value.Entity.Comment,
                    FromTime = keyValuePair.Value.Value.Entity.FromTime,
                    ToTime = keyValuePair.Value.Value.Entity.ToTime,
                    IdProjectItemRate = keyValuePair.Value.Value.Entity.IdProjectItemRate.Value,
                }, keyValuePair.Key));
                if (apiResult.Success)
                {
                    Editors.Remove(keyValuePair.Key);
                    //Model.WorksheetItems.Add(apiResult.Object);
                    var worksheetItemModel = keyValuePair.Value.Key;
                    worksheetItemModel.FromTime = apiResult.Object.FromTime;
                    worksheetItemModel.ToTime = apiResult.Object.ToTime;
                    worksheetItemModel.Comment = apiResult.Object.Comment;
                    worksheetItemModel.IdProjectItemRate = apiResult.Object.IdProjectItemRate;
                }

                //Editors.Remove(keyValuePair.Key);
            }
            ServerErrorManager.DisplayStatus();
        }
    }

    private void CheckOverlaps(IEnumerable<KeyValuePair<Guid, WorksheetItemEditorBatchEdit>> wsItems)
    {
        foreach (var modifiedItem in wsItems)
        {
            modifiedItem.Value.Overlaps = false;
            var others = Model.WorksheetItems.Where(e => e.DateOfAction.Date == modifiedItem.Value.DateOfAction.Date
                                                         && e.WorksheetItemId != modifiedItem.Key);
            foreach (var other in others)
            {
                if (!DateInfo.IsHourInRangeOf(
                        new Tuple<int, int>(modifiedItem.Value.FromTime, modifiedItem.Value.ToTime),
                        new Tuple<int, int>(other.FromTime, other.ToTime)))
                {
                    continue;
                }

                modifiedItem.Value.Overlaps = true;
                var otherItem = StartEditing(other);
                otherItem.Overlaps = true;
            }
        }
    }

    private void RevertDay(DateTime dayDate)
    {
        foreach (var keyValuePair in Editors.Where(e => e.Value.Value.Entity.DateOfAction.Date == dayDate).ToArray())
        {
            Editors.Remove(keyValuePair.Key);
        }
    }

    private string FormatChargeRateDisplay(ProjectItemRateViewModel rate)
    {
        if (PresentationModeService.PresentationState?.Enabled == true)
        {
            return rate.Name;
        }

        return $"{rate.Name} - {rate.Rate} ({rate.TaxRate}%)";
    }
}