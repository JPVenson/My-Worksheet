using System;
using MyWorksheet.Website.Server.Services.ServerInfo;
using MyWorksheet.Website.Server.Services.ServerManager;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace MyWorksheet.Webpage.AppStartup;

public static class ServerListenersConfig
{
    public static IApplicationBuilder RegisterServerListeners(IApplicationBuilder app)
    {
        var infoManager = app.ApplicationServices.GetService<ServerInfoManager>();
        //serverInfoManager.ExternalListeners.Add((publishValue) => new DbIntegrityListener(publishValue));
        infoManager.ExternalListeners.Add(pushValue => new ServerCapabilityListener(pushValue, app.ApplicationServices.GetService<IServerManagerService>()));
        return app;
    }
}

public class ServerCapabilityListener : ServerInfoListener
{
    private readonly IServerManagerService _serverManagerService;

    public ServerCapabilityListener(Action<string, object> pushValue, IServerManagerService serverManagerService) : base(pushValue)
    {
        _serverManagerService = serverManagerService;
    }

    public override string Key { get; } = "ServerCapabilities";
    public override void PublishValue()
    {
        base.Publish(_serverManagerService.Self.ServerCapabilities);
    }
}