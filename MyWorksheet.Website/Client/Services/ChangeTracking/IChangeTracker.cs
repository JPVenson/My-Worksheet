using System;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Signal;

namespace MyWorksheet.Website.Client.Services.ChangeTracking;

public interface IChangeTracker : IDisposable
{
    string TrackingType { get; }
    Task Invoke(EntityChangedEventArguments arguments);
}

public class NoneChangeTracker : IChangeTracker
{
    public void Dispose()
    {

    }

    public string TrackingType { get; }
    public Task Invoke(EntityChangedEventArguments arguments)
    {
        return Task.CompletedTask;
    }
}