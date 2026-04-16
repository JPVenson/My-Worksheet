using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Base;

public class TabHeaderModel : ComponentBase
{
    [CascadingParameter(Name = "TabParent")]
    public TabHeader TabParent { get; set; }

    [Parameter]
    [Required]
    public string Key { get; set; }

    [Parameter]
    public string HeaderContent { get; set; }

    [Parameter]
    public RenderFragment HeaderTemplate { get; set; }

    protected override void OnInitialized()
    {
        TabParent.Tabs.Add(this);
        TabParent.Render();
        base.OnInitialized();
    }
}