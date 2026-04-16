using System;
using System.Threading.Tasks;

namespace MyWorksheet.Website.Client.Pages.Base;

public class AsyncDisposable : IAsyncDisposable
{
    private readonly Func<ValueTask> _action;

    public AsyncDisposable() : this(() => ValueTask.CompletedTask)
    {

    }

    public AsyncDisposable(Func<ValueTask> action)
    {
        _action = action;
    }

    public ValueTask DisposeAsync()
    {
        return _action();
    }
}