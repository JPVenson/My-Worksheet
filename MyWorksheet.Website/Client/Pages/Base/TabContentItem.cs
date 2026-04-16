using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Base;

public class TabContentItem : ComponentBase
{
    [CascadingParameter(Name = "TabParent")]
    public TabContent ParentTabContent { get; set; }

    [Parameter]
    [Required]
    public string Key { get; set; }

    [Parameter]
    public bool IsDefault { get; set; }

    [Parameter]
    [Required]
    public RenderFragment ChildContent { get; set; }

    protected override void OnInitialized()
    {
        ParentTabContent.TabItems.Add(this);
        ParentTabContent.Render();
        base.OnInitialized();
    }
}