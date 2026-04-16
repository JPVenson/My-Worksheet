using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.LocalStorage;
using MyWorksheet.Website.Server.Services;
using MyWorksheet.Website.Shared.Services.Activation;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Client.Services.Auth;

[SingletonService()]
public class CurrentUserStore : RequireInit, ILazyLoadedService
{
    private readonly HttpService _httpService;
    private readonly ActivatorService _activatorService;
    public StorageService StorageService { get; }

    public CurrentUserStore(StorageService storageService, HttpService httpService, ActivatorService activatorService)
    {
        _httpService = httpService;
        _activatorService = activatorService;
        StorageService = storageService;
        AuthHandlers = new List<AuthHandler>();
    }

    public override async ValueTask InitAsync()
    {
        CurrentToken = await StorageService.LoginToken.ReadValue();
        if (CurrentToken != null)
        {
            _httpService.SetToken(CurrentToken.Token);
            OnDataLoaded();
            await AuthenticationChanged();
        }
    }

    public LoginResult CurrentToken { get; private set; }

    public class AuthHandler : IDisposable
    {
        private readonly CurrentUserStore _store;

        public AuthHandler(CurrentUserStore store)
        {
            _store = store;
            _store.AuthHandlers.Add(this);
            WhenAuthenticated = new List<Delegate>();
            WhenUnauthenticated = new List<Delegate>();
        }

        private IList<Delegate> WhenAuthenticated { get; set; }
        private IList<Delegate> WhenUnauthenticated { get; set; }

        public AuthHandler UserIsAuthenticated(Action action)
        {
            WhenAuthenticated.Add(action);
            return this;
        }

        public AuthHandler UserIsNotAuthenticated(Action action)
        {
            WhenUnauthenticated.Add(action);
            return this;
        }

        public AuthHandler UserIsAuthenticated(Func<Task> action)
        {
            WhenAuthenticated.Add(action);
            return this;
        }

        public AuthHandler UserIsAuthenticated(Func<ValueTask> action)
        {
            WhenAuthenticated.Add(action);
            return this;
        }

        public AuthHandler UserIsNotAuthenticated(Func<Task> action)
        {
            WhenUnauthenticated.Add(action);
            return this;
        }

        public AuthHandler UserIsNotAuthenticated(Func<ValueTask> action)
        {
            WhenUnauthenticated.Add(action);
            return this;
        }

        public IEnumerable<Delegate> GetInvocationList()
        {
            if (_store.CurrentToken != null)
            {
                return WhenAuthenticated;
            }
            return WhenUnauthenticated;
        }

        public Task Invoke()
        {
            if (_store.CurrentToken != null)
            {
                return _store.InvokeHandlers(WhenAuthenticated);
            }
            return _store.InvokeHandlers(WhenUnauthenticated);
        }

        public void Dispose()
        {
            _store.AuthHandlers.Remove(this);
            WhenAuthenticated.Clear();
            WhenUnauthenticated.Clear();
        }
    }

    public IList<AuthHandler> AuthHandlers { get; set; }

    public AuthHandler WhenChanged()
    {
        return new AuthHandler(this);
    }

    private Task AuthenticationChanged()
    {
        var invokerList = AuthHandlers.SelectMany(f => f.GetInvocationList());
        return InvokeHandlers(invokerList);
    }

    private async Task InvokeHandlers(IEnumerable<Delegate> invokerList)
    {
        foreach (var @delegate in invokerList)
        {
            var activateMethod = _activatorService.ActivateMethod(@delegate, @delegate.Target);
            if (activateMethod is Task t)
            {
                await t;
            }
            if (activateMethod is ValueTask tx)
            {
                await tx;
            }
        }
    }

    public async Task SetToken(LoginResult state)
    {
        CurrentToken = state;
        OnDataLoaded();
        await StorageService.LoginToken.WriteValue(state);
        await AuthenticationChanged();
    }

    public LoginResult GetAccount()
    {
        return CurrentToken;
    }

    public bool HasRole(string role)
    {
        if (CurrentToken?.UserData?.PageSettings?.Roles == null)
        {
            return false;
        }

        if (CurrentToken?.UserData?.PageSettings?.IsAdmin == true)
        {
            return true;
        }

        return CurrentToken?.UserData?.PageSettings?.Roles.Any(f => f.Equals(role, StringComparison.InvariantCultureIgnoreCase)) == true;
    }

    public event EventHandler DataLoaded;

    protected virtual void OnDataLoaded()
    {
        DataLoaded?.Invoke(this, EventArgs.Empty);
    }
}