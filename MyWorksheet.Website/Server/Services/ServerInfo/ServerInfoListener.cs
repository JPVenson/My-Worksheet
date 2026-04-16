using System;

namespace MyWorksheet.Website.Server.Services.ServerInfo;

public abstract class ServerInfoListener : IDisposable
{
    protected ServerInfoListener(Action<string, object> publishValue)
    {
        _publishValue = publishValue;
    }

    public abstract string Key { get; }

    private readonly Action<string, object> _publishValue;

    public virtual void Publish<T>(T value)
    {
        _publishValue(Key, value);
        //AccessElement<ServerInfoHubInfo>.Instance.SendServerInfoHubChanged(Key, value);
    }

    protected virtual void OnDispose()
    {

    }

    public void Dispose()
    {
        OnDispose();
    }

    public abstract void PublishValue();
}