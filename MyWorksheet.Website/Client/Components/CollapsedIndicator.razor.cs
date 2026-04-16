using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Collapse;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Components;

public partial class CollapsedIndicator : IDisposable
{
    [Inject]
    public CollapseService CollapseService { get; set; }

    [Parameter]
    public string Key { get; set; }

    public bool State { get; set; }

    [Parameter]
    public string Class { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object> Attributes { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        AddDisposable(CollapseService.WhenChanged(Key, StateHasChanged));
        State = CollapseService.GetState(Key);
    }

    private void StateHasChanged(bool fromvalue, bool tovalue, string key)
    {
        State = CollapseService.GetState(Key);
        StateHasChanged();
    }
}