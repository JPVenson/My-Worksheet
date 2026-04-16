using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Navigation;
using MyWorksheet.Website.Client.Util.View;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Base;

public partial class TabHeader
{
    public TabHeader()
    {
        Tabs = new List<TabHeaderModel>();
    }

    [Inject]
    public NavigationService NavigationService { get; set; }

    [Parameter]
    public TabbedNavigationPageBase TabView { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; }

    [Parameter]
    public string TabKey { get; set; }

    [Inject]
    public TabViewService TabViewService { get; set; }

    public IList<TabHeaderModel> Tabs { get; set; }

    public string CurrentTab
    {
        get => field;
        set
        {
            SetProperty(ref field, value, CurrentTabChanged);
            System.Console.WriteLine($"TabHeader [{TabKey}] - Current [{value}]");
        }
    }

    public EventCallback<string> CurrentTabChanged { get; set; }

    private void SetPage(TabHeaderModel header)
    {
        CurrentTab = header.Key;
        if (TabView is not null)
        {
            TabView.CurrentPage = header.Key;
        }

        var tabContent = TabViewService.Tabs[TabKey];
        tabContent.CurrentTab = tabContent.TabItems.FirstOrDefault(e => e.Key == CurrentTab);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        CurrentTab = TabView?.CurrentPage;
        if (CurrentTab is null)
        {
            System.Console.WriteLine("Register Tab listener");
            ChangeAdapter tabServiceAdapter = null;
            tabServiceAdapter = WhenChanged(TabViewService).Then(() =>
            {
                if (CurrentTab is null)
                {
                    TabContent tabContent = TabViewService.Tabs[TabKey];
                    var currentTab = tabContent.CurrentTab;
                    if (currentTab is not null)
                    {
                        CurrentTab = currentTab.Key;
                        tabServiceAdapter.Dispose();
                    }
                    else
                    {
                        ChangeAdapter tabContentAdapter = null;
                        tabContentAdapter = WhenChanged(tabContent).Then(() =>
                        {
                            if (tabContent.CurrentTab is not null)
                            {
                                CurrentTab = tabContent.CurrentTab.Key;
                                tabContentAdapter.Dispose();
                                tabServiceAdapter.Dispose();
                            }
                        });
                    }
                }
            });
        }
    }
}