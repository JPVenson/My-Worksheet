using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Settings.UserSettings;

public partial class UserChangePasswordComponent
{
    public UserChangePasswordComponent()
    {
        Model = new ChangePasswordModel();
    }

    public ChangePasswordModel Model { get; set; }

    [Inject]
    public HttpService HttpService { get; set; }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
    }

    public async Task Save()
    {
        ServerErrorManager.Clear();
        using (WaiterService.WhenDisposed())
        {
            var apiResult = ServerErrorManager.Eval(await HttpService.AccountApiAccess.ChangePassword(new AccountApiUserChangePassword()
            {
                OldPassword = Model.OldPassword,
                NewPassword = Model.NewPassword,
                ConfirmPassword = Model.NewPasswordConfirm
            }));
            if (apiResult.Success)
            {
                Model = new ChangePasswordModel();
            }
            ServerErrorManager.DisplayStatus();
        }
    }
}

public class ChangePasswordModel
{
    [Required]
    public string OldPassword { get; set; }

    [Required]
    public string NewPassword { get; set; }

    [Required]
    [Compare(nameof(NewPassword))]
    public string NewPasswordConfirm { get; set; }
}