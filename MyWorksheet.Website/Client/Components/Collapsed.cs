using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Services.Collapse;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace MyWorksheet.Website.Client.Components;

public class Collapsed : ComponentViewBase
{
    public Collapsed()
    {
        ElementType = "div";
        UseDomManipulation = true;
    }

    private bool _isCollapsed;

    [Inject]
    public CollapseService CollapseService { get; set; }

    [Inject]
    public IJSRuntime JsRuntime { get; set; }

    [Parameter]
    public string Class { get; set; }

    [Parameter]
    [Required]
    public string Key { get; set; }

    public bool IsCollapsedState { get; set; }

    [Parameter]
    public string ElementType { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object> Attributes { get; set; }

    [Parameter]
    public bool UseDomManipulation { get; set; }

    [Parameter]
    public bool IsCollapsed
    {
        get { return _isCollapsed; }
        set
        {
            if (SetProperty(ref _isCollapsed, value, IsCollapsedChanged))
            {
                if (!CollapseService.SetState(Key, value))
                {
                    _isCollapsed = !value;
                }
            }
        }
    }

    [Parameter]
    public EventCallback<bool> IsCollapsedChanged { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; }

    private bool _initState = false;

    protected override void OnInitialized()
    {
        AddDisposable(CollapseService.WhenChanged(Key, StateHasChanged));
        _initState = _isCollapsed = IsCollapsedState = CollapseService.GetState(Key);
        base.OnInitialized();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);
        builder.OpenElement(0, ElementType);
        builder.AddAttribute(1, "collapse-key", Key);
        builder.AddAttribute(2, "class", "collapse " + Class + (_initState ? " show" : ""));
        builder.AddAttribute(3, "id", "collapsable_" + Key);
        builder.AddMultipleAttributes(4, Attributes);
        if (IsCollapsedState || !UseDomManipulation)
        {
            builder.AddContent(5, ChildContent);
        }
        builder.CloseElement();
    }

    private async void StateHasChanged(bool fromvalue, bool tovalue, string key, Action stateChanged)
    {
        _initState = false;
        if (tovalue)
        {
            IsCollapsedState = true;
            Render();
            OnContinuesRender(async () =>
            {
                await JsRuntime.InvokeVoidAsync("MyWorksheet.Blazor.SetCollapsedAction", Key, true);
                stateChanged();
            });
        }
        else
        {
            await JsRuntime.InvokeVoidAsync("MyWorksheet.Blazor.SetCollapsedAction", Key, false);
            await Task.Delay(TimeSpan.FromSeconds(.5))
                .ContinueWith(t =>
                {
                    IsCollapsedState = false;
                    InvokeAsync(Render);
                    stateChanged();
                });
        }
    }
}