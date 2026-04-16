using System;
using System.Collections.Generic;
using MyWorksheet.Website.Client.Services.Collapse;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.TimeTracking.Worksheet.Display.PerWeek;

public partial class TimeProgressComponent : IDisposable
{
    private IDisposable _collapseHandler;

    [Inject]
    public CollapseService CollapseService { get; set; }

    [Parameter]
    public IEnumerable<WorksheetItemModel> WorksheetItems { get; set; }

    [Parameter]
    public string CollapseState { get; set; }

    protected override void OnInitialized()
    {
        _collapseHandler = CollapseService.WhenChanged(CollapseState, CollapsedStateChanged);
    }

    private void CollapsedStateChanged(bool fromvalue, bool tovalue, string key)
    {
        if (CollapseState != key)
        {
            return;
        }

        StateHasChanged();
    }

    public void Dispose()
    {
        _collapseHandler?.Dispose();
    }
}