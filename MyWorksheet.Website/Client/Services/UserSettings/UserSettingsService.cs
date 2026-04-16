using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Auth;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Workflow;
using MyWorksheet.Website.Server.Services;
using MyWorksheet.Website.Shared.Services.Activation;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.UserSettings;
using Newtonsoft.Json.Linq;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Client.Services.UserSettings;

[SingletonService()]
public class UserSettingsService : RequireInit, ILazyLoadedService
{
    private readonly CurrentUserStore _currentUserStore;
    private readonly HttpService _httpService;

    public UserSettingsService(CurrentUserStore currentUserStore, HttpService httpService)
    {
        _currentUserStore = currentUserStore;
        _httpService = httpService;
        UiSettings = new WorksheetUiOptions.ClientSettings();
    }

    public override async ValueTask InitAsync()
    {
        await base.InitAsync();
        await _currentUserStore.WhenChanged()
            .UserIsAuthenticated(LoadUserData)
            .UserIsNotAuthenticated(() => UiSettings = new WorksheetUiOptions.ClientSettings())
            .Invoke();
    }

    public WorksheetUiOptions.ClientSettings UiSettings { get; set; }

    private async Task LoadUserData()
    {
        var apiResult =
            await _httpService.UserAppSettingsApiAccess.GetSetting("BrowserLocalSettings", "BrowserLocalSettings");
        if (apiResult.Success)
        {
            UiSettings = (apiResult.Object as JToken)?.ToObject<WorksheetUiOptions.ClientSettings>();
            OnDataLoaded();
        }
    }

    public ValueTask<ApiResult> SaveUiSettings()
    {
        return _httpService.UserAppSettingsApiAccess.UpdateSetting("BrowserLocalSettings", "BrowserLocalSettings",
            UiSettings);
    }

    public event EventHandler DataLoaded;

    protected virtual void OnDataLoaded()
    {
        DataLoaded?.Invoke(this, EventArgs.Empty);
    }
}