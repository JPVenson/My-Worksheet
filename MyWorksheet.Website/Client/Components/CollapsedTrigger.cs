using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Services.Collapse;
using MyWorksheet.Website.Client.Util.View;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace MyWorksheet.Website.Client.Components;

public class CollapsedTrigger : ComponentViewBase, IDisposable
{
    public CollapsedTrigger()
    {
        ElementType = "div";
    }

    [Parameter]
    public bool IsDisabled { get; set; }
    [Parameter]
    public bool IsCollapsedDisabled { get; set; }

    [Inject]
    public CollapseService CollapseService { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; }

    [Parameter]
    [Required]
    public string Key { get; set; }

    [Parameter]
    public string ElementType { get; set; }

    public bool State { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object> Attributes { get; set; }

    [Parameter]
    public EventCallback OnClick { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        AddDisposable(CollapseService.WhenChanged(Key, StateHasChanged));
        State = CollapseService.GetState(Key);
    }

    private void StateHasChanged(bool fromvalue, bool tovalue, string key)
    {
        State = CollapseService.GetState(Key);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);
        builder.OpenElement(0, ElementType);
        builder.AddMultipleAttributes(1, Attributes);
        builder.AddAttribute(2, "onclick", EventCallback.Factory.Create(this, ToggleState));
        builder.AddContent(3, ChildContent);
        builder.CloseElement();
    }

    private async Task ToggleState()
    {
        if (IsDisabled)
        {
            return;
        }

        if (IsCollapsedDisabled)
        {
            await OnClick.RaiseAsync();
        }
        else
        {
            State = CollapseService.GetState(Key);
            if (CollapseService.SetState(Key, !State))
            {
                await OnClick.RaiseAsync();
            }
        }


        StateHasChanged();
    }
}