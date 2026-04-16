
using System;
using System.Collections.Generic;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Shared.ViewModels;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Client.Services.Navigation;

[SingletonService()]
public class TabViewService : ViewModelBase
{
    public TabViewService()
    {
        Tabs = new Dictionary<string, TabContent>();
    }

    public IDictionary<string, TabContent> Tabs { get; set; }

    public IDisposable RegisterTabContent(string key, TabContent tabContent)
    {
        Tabs[key] = tabContent;
        SendPropertyChanged(() => Tabs);
        return new Disposable(() =>
        {
            Tabs.Remove(key);
        });
    }
}