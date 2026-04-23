using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Auth;
using MyWorksheet.Website.Client.Services.Navigation;
using MyWorksheet.Website.Client.Services.UserSettings;
using MyWorksheet.Website.Shared.Services.Activation;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using ServiceLocator.Attributes;
using Task = System.Threading.Tasks.Task;
using MyWorksheet.Website.Shared.Services;

namespace MyWorksheet.Website.Client.Services.Signal;

[SingletonService()]
public class SignalService : RequireInit
{
    private readonly CurrentUserStore _currentUserStore;
    private readonly NavigationService _navigationService;

    public SignalService(CurrentUserStore currentUserStore,
        NavigationService navigationService)
    {
        _currentUserStore = currentUserStore;
        _navigationService = navigationService;
        HubState = new HubState() { IsConnected = true };
    }

    private IDictionary<ISignalHub, HubConnection> Hubs { get; set; }

    public ObjectChangedHub ObjectChangedHub { get; set; }
    public LoggerEntryHub LoggerEntryHub { get; set; }

    public HubState HubState { get; private set; }

    public override async ValueTask InitAsync(IServiceProvider services)
    {
        await base.InitAsync();
        var activator = services.GetRequiredService<ActivatorService>();
        foreach (var hubPropertyInfo in GetType().GetProperties().Where(e => typeof(ISignalHub).IsAssignableFrom(e.PropertyType)))
        {
            hubPropertyInfo.SetValue(this, activator.ActivateType(hubPropertyInfo.PropertyType));
        }

        await _currentUserStore.WhenChanged()
            .UserIsAuthenticated(StartSignalRHub)
            .UserIsNotAuthenticated(StopSignalRHub)
            .Invoke();
    }

    private async Task StopSignalRHub()
    {
        if (Hubs == null)
        {
            return;
        }

        foreach (var hubConnection in Hubs)
        {
            hubConnection.Value.Closed -= Connection_Closed;
            hubConnection.Value.Reconnected -= Connection_Reconnected;
            await hubConnection.Value.StopAsync();
        }
        Hubs.Clear();
    }

    public class EndlessRetryPolicy : IRetryPolicy
    {
        internal static TimeSpan?[] DEFAULT_RETRY_DELAYS_IN_MILLISECONDS = new TimeSpan?[]
        {
            TimeSpan.Zero,
            TimeSpan.FromSeconds(2),
            TimeSpan.FromSeconds(2),
            TimeSpan.FromSeconds(10),
            TimeSpan.FromSeconds(30),
        };

        private TimeSpan?[] _retryDelays;

        public EndlessRetryPolicy()
        {
            _retryDelays = DEFAULT_RETRY_DELAYS_IN_MILLISECONDS;
        }

        public EndlessRetryPolicy(TimeSpan[] retryDelays)
        {
            _retryDelays = new TimeSpan?[retryDelays.Length + 1];

            for (int i = 0; i < retryDelays.Length; i++)
            {
                _retryDelays[i] = retryDelays[i];
            }
        }

        public TimeSpan? NextRetryDelay(RetryContext retryContext)
        {
            if (retryContext.PreviousRetryCount >= _retryDelays.Length)
            {
                return TimeSpan.FromSeconds(30);
            }
            var retry = _retryDelays[retryContext.PreviousRetryCount];
            return retry;
        }
    }

    private async Task StartSignalRHub()
    {
        void ConfigHub(HttpConnectionOptions options)
        {
            options.AccessTokenProvider = () =>
            {
                var currentTokenToken = _currentUserStore?.CurrentToken?.Token;
                return Task.FromResult(currentTokenToken);
            };
        }

        Hubs =
            GetType()
                .GetProperties()
                .Where(e => typeof(ISignalHub).IsAssignableFrom(e.PropertyType))
                .Select(f => f.GetValue(this) as ISignalHub)
                .Where(e => e != null)
                .Where(e => e.CanConnect(_currentUserStore))
                .ToArray()
                .ToDictionary(e => e, e => (HubConnection)null);

        foreach (var hub in Hubs.Keys.ToArray())
        {
            var connection = new HubConnectionBuilder()
                .AddNewtonsoftJsonProtocol()
                .WithAutomaticReconnect(new EndlessRetryPolicy())
                .WithUrl(_navigationService.NavigationManager.ToAbsoluteUri(Path.Combine("/hubs/", hub.HubName)), ConfigHub)
                .Build();
            connection.Closed += Connection_Closed;
            connection.Reconnected += Connection_Reconnected;

            Hubs[hub] = connection;
            hub.Register(connection);
        }

        foreach (var hubConnection in Hubs)
        {
            await hubConnection.Value.StartAsync();
        }

        foreach (var hub in Hubs)
        {
            await hub.Key.Init(hub.Value);
        }

        await OnHubStateChanged();
    }

    private async Task Connection_Reconnected(string arg)
    {
        foreach (var hubConnection in Hubs)
        {
            await hubConnection.Key.OnReconnect(hubConnection.Value);
        }
        await OnHubStateChanged();
    }

    private Task Connection_Closed(Exception arg)
    {
        return OnHubStateChanged();
    }

    public event Func<HubState, Task> HubStateChanged;

    protected virtual Task OnHubStateChanged()
    {
        HubState = new HubState()
        {
            IsConnected = Hubs.Values.All(f => f.State == HubConnectionState.Connected)
        };

        return HubStateChanged?.Invoke(HubState) ?? Task.CompletedTask;
    }
}