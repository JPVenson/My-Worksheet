using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Navigation;
using MyWorksheet.Website.Shared.Services.Activation;
using MyWorksheet.Website.Shared.Services.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Client.Services.Auth;

[ScopedService(typeof(AuthenticationStateProvider))]
public class ClientAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly CurrentUserStore _store;
    private readonly HttpService _httpService;
    private readonly NavigationService _navigationManager;

    public ClientAuthenticationStateProvider(CurrentUserStore store, HttpService httpService, NavigationService navigationManager)
    {
        _store = store;
        _httpService = httpService;
        _navigationManager = navigationManager;
        _connections = new List<HubConnection>();
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = _store.GetAccount();

        if (token != null)
        {
            var tokenValue = JwtCoder.DecodeToken(token.Token);
            var claimsIdentity = new ClaimsIdentity(tokenValue.Claims, "bearer", ClaimTypes.NameIdentifier, ClaimTypes.Role);
            return new AuthenticationState(new ClaimsPrincipal(claimsIdentity));
        }

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(new Claim[0], null)));
    }

    private IList<HubConnection> _connections;

    public async Task SetState(LoginResult state, bool invokeEvent = false)
    {
        _httpService.SetToken(state?.Token);
        if (state != null)
        {
            var userCall = (await _httpService.UserApiAccess.GetCurrentUserData());
            if (!userCall.Success)
            {
                await _store.SetToken(null);
            }
            else
            {
                state.UserData = userCall.Object;
                await _store.SetToken(state);
            }
        }
        else
        {
            await _store.SetToken(null);
        }

        if (invokeEvent)
        {
            base.NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }

    public async Task Logoff()
    {
        await SetState(null, true);
        _navigationManager.NavigateTo("/", true);
    }

    public void RegisterHub(HubConnection hubConnection)
    {
        _connections.Add(hubConnection);
    }
}