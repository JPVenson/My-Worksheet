using System;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Auth;
using MyWorksheet.Website.Client.Util;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;
using Microsoft.AspNetCore.SignalR.Client;

namespace MyWorksheet.Website.Client.Services.Signal;

public class LoggerEntryHub : SignalHubBase
{
    public LoggerEntryHub()
    {
        HubName = "LoggerHub";
        LogEvent = new PubSubEvent<AppLoggerLogViewModel[]>();
    }

    public PubSubEvent<AppLoggerLogViewModel[]> LogEvent { get; set; }

    public override bool CanConnect(CurrentUserStore currentUserStore)
    {
        return currentUserStore.HasRole("Administrator");
    }

    public override void Register(HubConnection connection)
    {
        connection.On<AppLoggerLogViewModel[]>("LogEntries", async models =>
        {
            await LogEvent.Raise(models);
        });
    }

    public Task<IAsyncDisposable> BeginLogging(Func<AppLoggerLogViewModel[], Task> newLogEntries)
    {
        PubSubEvent.TrackableDisposable trackableDisposable = null;
        return RegisterGroup("Logging", async () =>
        {
            await Connection.InvokeAsync("RegisterLog");
            trackableDisposable = LogEvent.Register(newLogEntries);
        }, async () =>
        {
            trackableDisposable.Dispose();
            await Connection.InvokeAsync("UnRegisterLog");
        });
    }
}