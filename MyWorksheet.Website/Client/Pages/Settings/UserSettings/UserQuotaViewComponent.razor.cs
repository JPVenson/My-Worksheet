using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.UserQuota;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Settings.UserSettings;

public partial class UserQuotaViewComponent
{
    [Inject]
    public UserQuotaService UserQuotaService { get; set; }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        WhenChanged(UserQuotaService).ThenRefresh(this);
        using (WaiterService.WhenDisposed())
        {
            await UserQuotaService.UserQuota.Load();
        }
    }
}