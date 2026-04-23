using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Services.Navigation;
using MyWorksheet.Website.Client.Util.View;
using MyWorksheet.Website.Shared.Services.Activation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using ServiceLocator.Attributes;
using MyWorksheet.Website.Shared.Services;

namespace MyWorksheet.Website.Client.Services.Module;

[SingletonService()]
public class ModuleService : RequireInit, ILazyLoadedService
{
    private readonly NavigationService _navManager;

    public ModuleService(NavigationService navManager)
    {
        Modules = new List<NavItem>();
        _navManager = navManager;
    }

    public void NavigateTo(string url)
    {
        _navManager.NavigateTo(url);
    }

    private NavItem _currentNavItem;
    private LocalizableString _currentTitle;
    public IList<NavItem> Modules { get; set; }

    public NavItem CurrentNavItem
    {
        get { return _currentNavItem; }
        set
        {
            if (value != _currentNavItem)
            {
                _currentNavItem = value;
                OnDataLoaded();
            }
        }
    }

    public LocalizableString CurrentTitle
    {
        get { return _currentTitle; }
        set
        {
            _currentTitle = value;
            OnDataLoaded();
        }
    }

    private static string _routeRegexPattern = @"{\*?({FFF})\??:?(\w*)?\??}";
    public string GetModuleLink(string url, object arguments = null)
    {
        IEnumerable<KeyValuePair<string, string>> args;

        if (arguments == null)
        {
            args = Enumerable.Empty<KeyValuePair<string, string>>();
        }
        else if (arguments is string[] strArgs)
        {
            args = strArgs.Select(f => f.Split("=")).Select(f => new KeyValuePair<string, string>(f[0], f[1]));
        }
        else
        {
            args = arguments.GetType().GetProperties().Select(f => new KeyValuePair<string, string>(f.Name, f.GetValue(arguments)?.ToString()));
        }

        foreach (var arg in args)
        {
            var pattern = _routeRegexPattern.Replace("{FFF}", arg.Key);
            url = Regex.Replace(url, pattern, arg.Value);
        }

        return url;
    }

    public override void Init()
    {
        foreach (var groupItem in GetGroups())
        {
            Modules.Add(groupItem);
        }

        foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(f => f.GetTypes())
                     .Where(e => e.IsClass && !e.IsAbstract)
                     .Where(e => typeof(NavigationPageBase).IsAssignableFrom(e)))
        {
            foreach (var route in type.GetCustomAttributes<RouteAttribute>())
            {
                var pageAttribute = type.GetCustomAttribute<PageAttribute>() ?? throw new NotSupportedException($"Please set a Page attribute for '{type}'");

                var module = new NavItem(pageAttribute.LocName, pageAttribute.GetName());
                module.Url = route.Template;
                module.Auth = type.GetCustomAttribute<AuthorizeAttribute>();
                module.IconName = pageAttribute.PageIconCss;
                module.ShowInNavbar = pageAttribute.IsNavbarView;
                if (pageAttribute.Group != null)
                {
                    var groupModule = Modules.FirstOrDefault(e => e.LocName == pageAttribute.Group);
                    groupModule.AddNavItem(module);
                }
                else
                {
                    Modules.Add(module);
                }
            }
        }
    }

    private const string DefaultAdminIcon = "fa fa-lock";

    private IEnumerable<NavItem> GetGroups()
    {
        yield return NavItem.Group("Links/StartInfo", "fas fa-home");
        yield return NavItem.Group("Links/Account", "fas fa-user-tie");
        yield return NavItem.Group("Links/Server", DefaultAdminIcon);
        yield return NavItem.Group("Links/Timeboard", "fa fa-business-time");
        yield return NavItem.Group("Links/Settings", "fa fa-cogs");
    }

    public void SetCurrent(string getName)
    {
        CurrentNavItem = Find(getName);
    }

    public event EventHandler DataLoaded;

    protected virtual void OnDataLoaded()
    {
        DataLoaded?.Invoke(this, EventArgs.Empty);
    }

    public NavItem Find(string name)
    {
        return Modules.SelectMany(f => f.MeAndMyChildren()).FirstOrDefault(e => e.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
    }
}

public class NavItem
{
    private readonly IList<NavItem> _children;

    public NavItem()
    {
        _children = new List<NavItem>();
    }

    public NavItem(string locName, string name, params NavItem[] children) : this()
    {
        LocName = locName;
        Name = name;
        ShowInNavbar = true;
        AddNavItem(children);
    }

    public NavItem(string locName, string name, string url, params NavItem[] children) : this(locName, name, children)
    {
        Url = url;
    }

    public string Name { get; set; }
    public string LocName { get; private set; }
    public string Url { get; set; }
    public NavItem Parent { get; private set; }
    public Action OnClick { get; set; }
    public object Tag { get; set; }

    public bool ShowInNavbar { get; set; }
    public bool ShowInBreadcrumbs { get; set; }

    public AuthorizeAttribute Auth { get; set; }

    public IEnumerable<NavItem> MeAndMyChildren()
    {
        yield return this;
        foreach (NavItem navItem in Children.SelectMany(f => f.MeAndMyChildren()))
        {
            yield return navItem;
        }
    }

    public IEnumerable<NavItem> Children
    {
        get
        {
            return _children;
        }
    }

    public string BadgeContent { get; set; }
    public string IconName { get; set; }

    public void AddNavItem(params NavItem[] navItem)
    {
        foreach (NavItem item in navItem)
        {
            item.Parent = this;
            item.Auth ??= Auth;
            _children.Add(item);
        }
    }

    public static NavItem Group(string displayName, string icon)
    {
        return new NavItem(displayName, PageAttribute.GetName(displayName))
        {
            IconName = icon,
            ShowInNavbar = true,
            ShowInBreadcrumbs = true
        };
    }
}

[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public sealed class PageAttribute : Attribute
{
    public PageAttribute(string pageLocName)
    {
        LocName = pageLocName;
    }

    public string Group { get; set; }
    public string LocName { get; set; }
    public bool IsNavbarView { get; set; }
    public string PageIconCss { get; set; }

    public string GetName()
    {
        return GetName(LocName);
    }

    public static string GetName(string locName)
    {
        var name = "";
        for (int i = 0; i < locName.Length; i++)
        {
            var c = locName[i];
            if (char.IsLetter(c))
            {
                name += c;
            }
            else
            {
                name += "_";
            }
        }

        return name;
    }
}