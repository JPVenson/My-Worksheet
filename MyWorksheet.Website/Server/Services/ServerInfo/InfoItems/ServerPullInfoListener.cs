using System;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;

namespace MyWorksheet.Website.Server.Services.ServerInfo.InfoItems;

public abstract class ServerPullInfoListener : ServerInfoListener
{
    public TaskTimer Consumers { get; protected set; }

    //public static Timer Timer1 { get; set; }

    protected ServerPullInfoListener(Action<string, object> publishValue, IAppLogger logger, int intervall = 1050) : base(publishValue)
    {
        Consumers = TaskTimerManager.GetOrCreateFromCache(intervall, nameof(ServerInfoManager), logger);
    }
}