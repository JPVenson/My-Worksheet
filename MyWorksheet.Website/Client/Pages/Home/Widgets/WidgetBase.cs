using System;
using MyWorksheet.Website.Client.Pages.Base;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Home.Widgets;

public abstract class WidgetViewBase : NavigationPageBase
{
    [Parameter]
    public DashboardWidgetInstance WidgetInstance { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        ValidateWidget();
    }

    protected virtual void ValidateWidget()
    {
        if (WidgetInstance == null)
        {
            throw new ArgumentNullException(nameof(WidgetInstance), "Widget parameter is required.");
        }
    }
}