using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Auth;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Settings.UserSettings;

public partial class UserSettingsEditComponent
{
    public EntityState<AccountApiUserGetInfo> UserData { get; set; }

    [Inject]
    public HttpService HttpService { get; set; }
    [Inject]
    public CurrentUserStore CurrentUserStore { get; set; }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        UserData = CurrentUserStore.CurrentToken.UserData.UserInfo;
    }

    public async Task Save()
    {
        ServerErrorManager.Clear();
        using (WaiterService.WhenDisposed())
        {
            var apiResult = ServerErrorManager.Eval(await HttpService.AccountApiAccess.UpdateUserData(UserData.Entity));
            if (apiResult.Success)
            {
                UserData.SetPristine();
                await CurrentUserStore.SetToken(CurrentUserStore.CurrentToken);
            }
            ServerErrorManager.DisplayStatus();
        }
    }
}