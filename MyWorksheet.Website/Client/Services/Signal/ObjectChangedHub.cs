using System;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Auth;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Util;
using MyWorksheet.Website.Shared.ViewModels;
using Microsoft.AspNetCore.SignalR.Client;

namespace MyWorksheet.Website.Client.Services.Signal;

public class ObjectChangedHub : SignalHubBase
{
    private readonly HttpService _httpService;

    public ObjectChangedHub(HttpService httpService)
    {
        _httpService = httpService;
        ExternalEntityChanged = new PubSubEvent<EntityChangedEventArguments>();
        HubName = "ObjectChangedHub";
    }

    public PubSubEvent<EntityChangedEventArguments> ExternalEntityChanged { get; set; }

    public override bool CanConnect(CurrentUserStore currentUserStore)
    {
        return currentUserStore.CurrentToken != null;
    }

    public override void Register(HubConnection connection)
    {
        connection.On<string, Guid[], ChangeEventTypes, Guid?>("ObjectChanged", async (s, i, arg3, arg4) => await ExternalEntityChanged.Raise(new EntityChangedEventArguments(s, i, arg3, arg4)));
    }

    public override async Task Init(HubConnection connection)
    {
        await base.Init(connection);
        await base.RegisterGroup("ChangeTracking", async () =>
        {
            var connectionId = await connection.InvokeAsync<string>("RegisterChangeTracking");
            _httpService.SignalChangeId(connectionId);
        }, async () =>
        {

            await connection.InvokeAsync("UnRegisterChangeTracking");
            _httpService.SignalChangeId(null);
        });
    }
}