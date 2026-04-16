using System;

namespace MyWorksheet.Website.Client.Services.Workflow;

public abstract class LazyLoadedService : ILazyLoadedService
{
    public event EventHandler DataLoaded;
    public event EventHandler<string> DataObjectLoaded;

    protected virtual void OnDataLoaded()
    {
        DataLoaded?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnDataLoaded(string key)
    {
        DataObjectLoaded?.Invoke(this, key);
    }
}