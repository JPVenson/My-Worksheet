using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.ThemeManager;
using MyWorksheet.Website.Client.Services.UserSettings;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Settings.UserSettings;

public partial class UserViewSettingsViewComponent
{
    [Inject]
    public UserSettingsService UserSettingsService { get; set; }

    [Inject]
    public ThemeManagerService ThemeManagerService { get; set; }

    public async Task SaveSettings()
    {
        ServerErrorManager.Clear();
        using (WaiterService.WhenDisposed())
        {
            ServerErrorManager.Eval(await UserSettingsService.SaveUiSettings());
            ServerErrorManager.DisplayStatus();
        }
    }
}