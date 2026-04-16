using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Components;
using MyWorksheet.Website.Client.Services.Module;
using MyWorksheet.Website.Client.Services.Navigation;
using MyWorksheet.Website.Client.Services.Workflow;
using MyWorksheet.Website.Client.Util.View;
using MyWorksheet.Website.Shared.Services.Activation;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Client.Services.Breadcrumb;

[SingletonService()]
public class BreadcrumbService : LazyLoadedService
{
    private readonly NavigationService _navigationService;
    private readonly ModuleService _moduleService;

    public BreadcrumbService(NavigationService navigationService, ModuleService moduleService)
    {
        _navigationService = navigationService;
        _moduleService = moduleService;
        Parts = new List<BreadcrumbPart>();
    }

    public IList<BreadcrumbPart> Parts { get; set; }

    public void Clear()
    {
        Parts.Clear();
        OnDataLoaded();
    }

    public BreadcrumbPart Add(LocalizableString title)
    {
        return Add(new BreadcrumbPart() { Title = title });
    }

    public BreadcrumbPart AddModuleLink(LocalizableString title, object arguments = null)
    {
        var module = _moduleService.Find(PageAttribute.GetName(title.Text));

        return Add(new BreadcrumbPart()
        {
            Title = title,
            Url = _moduleService.GetModuleLink(module.Url, arguments)
        });
    }

    public BreadcrumbPart Add(BreadcrumbPart part)
    {
        Parts.Add(part);
        OnDataLoaded();
        return part;
    }

    public void Remove(BreadcrumbPart part)
    {
        Parts.Remove(part);
        OnDataLoaded();
    }
}

public class BreadcrumbPart
{
    public string Path { get; set; }
    public LocalizableString Title { get; set; }
    public string Url { get; set; }
}