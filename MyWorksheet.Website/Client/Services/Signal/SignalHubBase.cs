using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Services.Auth;
using Microsoft.AspNetCore.SignalR.Client;

namespace MyWorksheet.Website.Client.Services.Signal;

public abstract class SignalHubBase : ISignalHub
{
    public SignalHubBase()
    {
        CurrentGroupRegistrations = new Dictionary<string, Tuple<Func<Task>, Func<Task>>>();
    }

    protected HubConnection Connection { get; private set; }

    public IDictionary<string, Tuple<Func<Task>, Func<Task>>> CurrentGroupRegistrations { get; set; }

    public async Task<IAsyncDisposable> RegisterGroup(string name, Func<Task> registerAction, Func<Task> unRegisteraction)
    {
        await registerAction();
        CurrentGroupRegistrations[name] = new Tuple<Func<Task>, Func<Task>>(registerAction, unRegisteraction);
        return new AsyncDisposable(async () =>
        {
            await unRegisteraction();
        });
    }

    public string HubName { get; protected set; }
    public virtual bool CanConnect(CurrentUserStore currentUserStore)
    {
        return true;
    }

    public virtual void Register(HubConnection connection)
    {
    }

    public virtual Task Init(HubConnection connection)
    {
        Connection = connection;
        return Task.CompletedTask;
    }

    public async Task OnReconnect(HubConnection connection)
    {
        foreach (var currentGroupRegistration in CurrentGroupRegistrations)
        {
            await currentGroupRegistration.Value.Item1();
        }
    }
}