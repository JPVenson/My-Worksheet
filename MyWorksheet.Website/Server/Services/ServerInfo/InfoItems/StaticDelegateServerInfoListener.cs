using System;

namespace MyWorksheet.Website.Server.Services.ServerInfo.InfoItems;

public class StaticDelegateServerInfoListener : ServerInfoListener
{
    private readonly Func<object> _getValue;

    public StaticDelegateServerInfoListener(Action<string, object> publishValue, string key, Func<object> getValue) : base(publishValue)
    {
        _getValue = getValue;
        Key = key;
    }

    public override string Key { get; }
    public override void PublishValue()
    {
        Publish(_getValue());
    }
}