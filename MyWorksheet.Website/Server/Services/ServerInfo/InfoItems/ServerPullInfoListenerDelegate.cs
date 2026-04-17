using System;

namespace MyWorksheet.Website.Server.Services.ServerInfo.InfoItems;

public class ServerPullInfoListenerDelegate : ServerPullInfoListener
{
    private readonly Func<object> _getValue;

    public ServerPullInfoListenerDelegate(Action<string, object> publisher, string key, Func<object> getValue,
        ILogger logger)
        : base(publisher, logger)
    {
        _getValue = getValue;
        Key = key;
        Consumers.Add(PublishValue);
    }

    protected override void OnDispose()
    {
        Consumers.Remove(PublishValue);
        base.OnDispose();
    }

    public override string Key { get; }
    public override void PublishValue()
    {
        Publish(_getValue());
    }
}