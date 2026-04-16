using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Components.Form;
using MyWorksheet.Website.Client.Util;
using MyWorksheet.Website.Client.Services.Auth;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.UserWorkload;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Address;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.UserWorkload;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Settings.UserSettings;

public partial class UserSettingsView
{
    [Inject]
    public CurrentUserStore CurrentUserStore { get; set; }

    [Inject]
    public HttpService HttpService { get; set; }

    public EntityState<AddressModel> UsersAddress { get; set; }
    public ServerErrorManager AddressErrors { get; set; }

    public EntityState<GetUserWorkloadViewModel> UserWorkload { get; set; }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        AddressErrors = new ServerErrorManager(WaiterService);
        var addressTask = HttpService.AddressApiAccess.MyAddress().AsTask();
        var workloadTask = HttpService.UserWorkloadApiAccess.GetDefaultWorkload().AsTask();
        await using (var tasks = new TaskList())
        {
            tasks.Add(addressTask);
            tasks.Add(workloadTask);
        }
        UsersAddress = AddressErrors.EvalAndUnbox(await addressTask);
        UserWorkload = ServerErrorManager.EvalAndUnbox(await workloadTask);

        WhenChanged(UsersAddress.Entity)
            .ThenRefresh(this);
        WhenChanged(UserWorkload.Entity)
            .ThenRefresh(this);
    }

    public async Task SaveUserWorkload()
    {
        if (!UserWorkload.IsObjectDirty)
        {
            return;
        }

        ServerErrorManager.Clear();
        using (WaiterService.WhenDisposed())
        {
            var apiResult = ServerErrorManager.Eval(await HttpService.UserWorkloadApiAccess.Update(UserWorkload.Entity));
            if (apiResult.Success)
            {
                UserWorkload = apiResult.Object;
            }
            ServerErrorManager.DisplayStatus();
        }
    }

    public async Task SaveAddress()
    {
        if (!UsersAddress.IsObjectDirty)
        {
            return;
        }

        AddressErrors.Clear();
        using (WaiterService.WhenDisposed())
        {
            var apiResult = AddressErrors
                .Eval(await HttpService.AddressApiAccess.Update(UsersAddress.Entity, UsersAddress.Entity.AddressId));
            if (apiResult.Success)
            {
                UsersAddress = apiResult.Object;
            }
            AddressErrors.DisplayStatus();
        }
    }
}