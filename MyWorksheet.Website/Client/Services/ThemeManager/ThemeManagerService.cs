using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Auth;
using MyWorksheet.Website.Client.Services.ResLoaded;
using MyWorksheet.Website.Client.Services.UI;
using MyWorksheet.Website.Server.Services;
using MyWorksheet.Website.Shared.Services.Activation;
using Microsoft.JSInterop;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Client.Services.ThemeManager;

[SingletonService()]
public class ThemeManagerService : RequireInit
{
    private readonly CurrentUserStore _currentUserStore;
    private readonly IJSRuntime _jsRuntime;
    private readonly ResourceLoaderService _resourceLoaderService;
    private readonly UIService _uiService;

    public ThemeManagerService(CurrentUserStore currentUserStore,
        IJSRuntime jsRuntime,
        ResourceLoaderService resourceLoaderService,
        UIService uiService)
    {
        _currentUserStore = currentUserStore;
        _jsRuntime = jsRuntime;
        _resourceLoaderService = resourceLoaderService;
        _uiService = uiService;

        _currentUserStore.WhenChanged()
            .UserIsAuthenticated(SetPreferedTheme)
            .Invoke();
        Themes = new[]
        {
            new Theme("theme-night", "Night", GetThemeUrl("/css/themes/dark/site-theme")),
            new Theme("theme-day", "Day", GetThemeUrl("/css/themes/light/site-theme")),
        };
        CurrentTheme = Themes.FirstOrDefault();
    }

    private string GetThemeUrl(string url)
    {
#if DEBUG
        return url + ".css";
#else
			return url + ".min.css";
#endif
    }

    private async ValueTask SetPreferedTheme()
    {
        await _uiService.UiLoaded.Register(async () =>
        {
            CurrentTheme = await GetPreferedTheme();

            if (CurrentThemeHandle != null)
            {
                await CurrentThemeHandle.DisposeAsync();
            }

            CurrentThemeHandle = await _resourceLoaderService.AddResource(new StyleLinkResource(CurrentTheme.SourceUrl), true);
        });
    }

    private async Task<Theme> GetPreferedTheme()
    {
        return await _jsRuntime.InvokeAsync<bool>("MyWorksheet.Blazor.GetThemeType")
            ? Themes.FirstOrDefault()
            : Themes.LastOrDefault();
    }

    /// <inheritdoc />
    public override async ValueTask InitAsync()
    {
        await SetPreferedTheme();
        await base.InitAsync();
    }

    public Theme[] Themes { get; set; }
    public Theme CurrentTheme { get; set; }
    public IAsyncDisposable CurrentThemeHandle { get; set; }
}

public class Theme
{
    public Theme(string key, string name, string sourceUrl)
    {
        Key = key;
        Name = name;
        SourceUrl = sourceUrl;
    }

    public string Key { get; set; }
    public string Name { get; set; }
    public string SourceUrl { get; set; }
}