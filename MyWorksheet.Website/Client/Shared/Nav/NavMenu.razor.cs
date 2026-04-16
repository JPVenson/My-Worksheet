using System.Collections.Generic;
using System.Linq;
using MyWorksheet.Website.Client.Services.Auth;
using MyWorksheet.Website.Client.Services.Module;
using MyWorksheet.Website.Client.Services.Navigation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace MyWorksheet.Website.Client.Shared.Nav;

public partial class NavMenu
{
    [Inject]
    public ModuleService ModuleService { get; set; }
    [Inject]
    public NavigationService NavigationService { get; set; }
    [Inject]
    public CurrentUserStore CurrentUserStore { get; set; }

    [Inject]
    public IWebAssemblyHostEnvironment WebAssemblyHostEnvironment { get; set; }

    public NavMenu()
    {
        NavItems = new List<NavItem>();
    }

    public IList<NavItem> NavItems { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        NavItems = ModuleService.Modules;
        WhenChanged(ModuleService).ThenRefresh(this);
    }

    public void OnNavbarActionAsync(NavItem navItem)
    {
        navItem.OnClick?.Invoke();
        var currentRelUrl = NavigationService.NavigationManager.ToBaseRelativePath(NavigationService.Uri);
        if (navItem.Url != null && navItem.Url != "/" + currentRelUrl)
        {
            NavigationService.NavigateTo(navItem.Url);
        }
    }

    protected string GetActiveNavItemClass(string name)
    {
        var currentNavItem = ModuleService.CurrentNavItem;
        if (currentNavItem != null)
        {
            var actives = new HashSet<NavItem>();
            actives.Add(currentNavItem);
            while (currentNavItem?.Parent != null)
            {
                actives.Add(currentNavItem.Parent);
                if (currentNavItem.Parent == null)
                {
                    break;
                }
                currentNavItem = currentNavItem.Parent;
            }

            if (actives.Any(e => e.LocName == name))
            {
                return "active";
            }
        }
        return "";
    }

    private bool FilterNavItem(NavItem arg)
    {
        if (!arg.ShowInNavbar)
        {
            return false;
        }

        if (arg.Url == null && arg.Children?.Any(FilterNavItem) == false)
        {
            return false;
        }

        if (CurrentUserStore.CurrentToken == null)
        {
            return arg.Auth == null;
        }

        if (arg.Auth?.Roles == null)
        {
            return true;
        }

        var roles = CurrentUserStore.CurrentToken.UserData.PageSettings.Roles;
        if (!arg.Auth.Roles.Split(",").Select(f => f.Trim()).All(e => roles.Contains(e)))
        {
            return false;
        }

        return true;
    }
}