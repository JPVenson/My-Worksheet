using System.Collections.Generic;

namespace MyWorksheet.Website.Client.Pages.Base;

public class TabManager
{
    public TabManager()
    {
        TabPages = new Dictionary<string, TabbedNavigationPageBase>();
    }

    public IDictionary<string, TabbedNavigationPageBase> TabPages { get; set; }
}