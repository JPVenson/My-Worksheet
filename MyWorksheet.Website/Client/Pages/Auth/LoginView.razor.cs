using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Auth;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Navigation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;

namespace MyWorksheet.Website.Client.Pages.Auth;

public partial class LoginView
{
    public LoginView()
    {
        LoginContext = new EditContext(LoginViewModel = new LoginViewModel());
    }

    [Inject]
    public HttpService HttpService { get; set; }

    [Inject]
    public AuthenticationStateProvider ClientAuthenticationStateProvider { get; set; }

    [Parameter]
    public string ReturnUrl { get; set; }

    public LoginViewModel LoginViewModel { get; set; }

    public EditContext LoginContext { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        var state = await ClientAuthenticationStateProvider.GetAuthenticationStateAsync();
        if (state.User.Identity.IsAuthenticated)
        {
            NavigationService.NavigateTo("/");
        }
    }

    public async Task Login()
    {
        ServerErrorManager.Clear();
        using (WaiterService.WhenDisposed())
        {
            var apiResult = ServerErrorManager.Eval(await HttpService.AuthApiAccess.Login(LoginViewModel.Username, LoginViewModel.Password, ""));
            if (apiResult.Success)
            {
                await (ClientAuthenticationStateProvider as ClientAuthenticationStateProvider).SetState(apiResult.Object, true);
                NavigationService.NavigateTo(ReturnUrl ?? "/");
            }
            ServerErrorManager.DisplayStatus();
        }
    }
}

public class LoginViewModel
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
}