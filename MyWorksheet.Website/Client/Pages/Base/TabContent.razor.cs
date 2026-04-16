using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Navigation;
using MyWorksheet.Website.Client.Util.View;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Base;

public partial class TabContent
{
    public TabContent()
    {
        TabItems = new List<TabContentItem>();
    }

    [Inject]
    public TabViewService TabViewService { get; set; }

    [Inject]
    public NavigationService NavigationService { get; set; }

    [Parameter]
    public TabbedNavigationPageBase TabView { get; set; }

    public IList<TabContentItem> TabItems { get; set; }

    private TabContentItem _currentTab;
    [Parameter]
    public TabContentItem CurrentTab
    {
        get { return _currentTab; }
        set
        {
            if (SetProperty(ref _currentTab, value, CurrentTabChanged) && value != null && TabView != null)
            {
                TabView.CurrentPage = value.Key;
            }

            StateHasChanged();
        }
    }

    [Parameter]
    public string TabKey { get; set; }

    [Parameter]
    public EventCallback<TabContentItem> CurrentTabChanged { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; }

    protected override void OnInitialized()
    {
        if (TabView is not null)
        {
            //TabView.CurrentPageChanged += TabPageChanged;
        }
        else
        {
            AddDisposable(TabViewService.RegisterTabContent(TabKey, this));
        }

        base.OnInitialized();
    }

    private void FragmentChanged()
    {
        Console.WriteLine("Fragment Change Detected");
        var fragment = NavigationService.QueryFragments.GetOrDefault("Whole", null);
        CurrentTab = TabItems.FirstOrDefault(e => e.Key == fragment) ?? CurrentTab;
        StateHasChanged();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        if (firstRender)
        {
            var fragment = NavigationService.QueryFragments.GetOrDefault("Whole", null);
            CurrentTab = TabItems.FirstOrDefault(e => e.Key == fragment) ??
                         TabItems.SingleOrDefault(f => f.IsDefault) ??
                         TabItems.FirstOrDefault();
            Render();
        }
    }

    private void TabPageChanged(object sender, string key)
    {
        CurrentTab = TabItems.FirstOrDefault(e => e.Key == key);
        StateHasChanged();
    }
}