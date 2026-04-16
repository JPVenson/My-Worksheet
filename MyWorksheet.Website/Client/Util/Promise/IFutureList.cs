using System;
using System.Threading.Tasks;

namespace MyWorksheet.Website.Client.Util.Promise;

public interface IFutureList
{
    event EventHandler ListLoaded;
    bool Loaded { get; }
    bool Loading { get; }

    IDisposable WhenLoaded(Action action);
    IDisposable WhenLoadedOnce(Action action);

    Task<IDisposable> WhenLoadedAsync(Func<Task> action);
    Task<IDisposable> WhenLoadedOnceAsync(Func<Task> action);
    Task Load();
    void Reset();
}