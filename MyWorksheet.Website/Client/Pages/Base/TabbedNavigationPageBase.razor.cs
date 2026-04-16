using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Util.View;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Base;

public abstract partial class TabbedNavigationPageBase : NavigationPageBase
{
    public TabbedNavigationPageBase()
    {
    }

    private string _currentPage;

    public string CurrentPage
    {
        get { return _currentPage; }
        set
        {
            SetProperty(ref _currentPage, value, CurrentPageChanged);
            PluginService.PluginsChanged.Raise();
        }
    }

    public event EventHandler<string> CurrentPageChanged;

    public override Task SetParametersAsync(ParameterView parameters)
    {
        CurrentPage = parameters.GetValueOrDefault<string>("Page");
        return base.SetParametersAsync(parameters);
    }
}