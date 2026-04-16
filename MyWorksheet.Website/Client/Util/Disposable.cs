using System;

namespace MyWorksheet.Website.Client.Pages.Base;

public class Disposable : IDisposable
{
    private readonly Action _action;

    public Disposable() : this(() => { })
    {

    }

    public Disposable(Action action)
    {
        _action = action;
    }

    public void Dispose()
    {
        _action();
    }
}