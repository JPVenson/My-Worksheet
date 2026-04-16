using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Services.UI;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace MyWorksheet.Website.Client.Shared;

public class NoLayoutLayoutLayout : ComponentViewBase
{
    public NoLayoutLayoutLayout()
    {

    }

    [Inject] public UIService UiService { get; set; }

    protected override void OnInitialized()
    {
        OnNextRender(() => UiService.UiLoaded.Raise().AsTask());
    }

    /// <summary>
    /// Gets the content to be rendered inside the layout.
    /// </summary>
    [Parameter]
    public RenderFragment Body { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);
        builder.AddContent(0, Body);
    }
}