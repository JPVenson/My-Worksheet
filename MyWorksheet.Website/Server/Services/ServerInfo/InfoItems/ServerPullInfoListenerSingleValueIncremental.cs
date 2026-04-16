using System;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;

namespace MyWorksheet.Website.Server.Services.ServerInfo.InfoItems;

public class ServerPullInfoListenerSingleValueIncremental<TValue> : ServerPullInfoListener
    where TValue : IComparable<TValue>
{
    private readonly Func<TValue, TValue> _getValue;

    public ServerPullInfoListenerSingleValueIncremental(Action<string, object> publisher, string key,
        Func<TValue, TValue> getValue, IAppLogger logger)
        : base(publisher, logger)
    {
        _getValue = getValue;
        Key = key;
        Consumers.Add(PublishValue);
    }

    private TValue _last;

    protected override void OnDispose()
    {
        Consumers.Remove(PublishValue);
        base.OnDispose();
    }

    public override string Key { get; }
    public override void PublishValue()
    {
        Publish(_last = _getValue(_last));
    }
}