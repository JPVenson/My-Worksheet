using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Auth;
using MyWorksheet.Website.Client.Services.Dialog;
using MyWorksheet.Website.Client.Services.Navigation;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Shared.Dialog;

public partial class LogoffDialog
{
    [Inject]
    public CurrentUserStore CurrentUserStore { get; set; }

    [Inject]
    public NavigationService NavigationService { get; set; }

    [Inject]
    public DialogService DialogService { get; set; }

    public async Task LogoffUser()
    {
        await CurrentUserStore.SetToken(null);
        await DialogService.Hide();
        NavigationService.NavigateTo("/Account/Logout", true);
    }
}