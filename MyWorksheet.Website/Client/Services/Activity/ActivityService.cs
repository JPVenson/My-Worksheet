using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Auth;
using MyWorksheet.Website.Client.Services.ChangeTracking;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Module;
using MyWorksheet.Website.Client.Services.Repository;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Shared.Services.Activation;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;
using Microsoft.AspNetCore.Components;
using ServiceLocator.Attributes;
using MyWorksheet.Website.Shared.Services;

namespace MyWorksheet.Website.Client.Services.Activity;

[SingletonService()]
public class ActivityService : RequireInit, ILazyLoadedService
{
    private readonly ActivatorService _activatorService;
    private readonly Dictionary<string, Delegate> _transform;

    public ActivityService(ICacheRepository<UserActivityViewModel> activitiesCache, ActivatorService activatorService)
    {
        _activatorService = activatorService;
        ActivitiesCache = activitiesCache;

        _transform = new Dictionary<string, Delegate>();
        _transform["goto"] = new Func<string[], ModuleService, string>(static (strings, moduleService) =>
        {
            var pageKey = strings[1];
            var arguments = strings[2];

            var navItem = moduleService.Find(PageAttribute.GetName(pageKey));
            if (navItem == null)
            {
                return pageKey;
            }

            return $"<a href='{moduleService.GetModuleLink(navItem.Url, arguments.Split("&"))}'></a>";
        });
    }

    public MarkupString Transform(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return new MarkupString();
        }

        var replace = new Regex("{{.*}}");
        foreach (Match match in replace.Matches(content))
        {
            var strings = match.Value.Trim('{', '}').Split(":");
            var name = strings.First();
            var transformator = _transform[name];
            var newValue = _activatorService.ActivateMethod(transformator, transformator.Target, new object[] { strings }) as string;
            content = content.Replace(match.Value, newValue);
        }

        return new MarkupString(content);
    }

    public ICacheRepository<UserActivityViewModel> ActivitiesCache { get; set; }

    public event EventHandler DataLoaded;

    protected virtual void OnDataLoaded()
    {
        DataLoaded?.Invoke(this, EventArgs.Empty);
    }
}