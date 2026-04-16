using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Organisation;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

namespace MyWorksheet.Website.Client.Util.Promise;

public class TrackedList<TValue> : Collection<TValue>, IFutureTrackedList<TValue> where TValue : IEntityObject
{
    public event EventHandler ListLoaded;
    public bool Loaded
    {
        get
        {
            return false;
        }
    }
    public bool Loading
    {
        get
        {
            return false;
        }
    }

    public IDisposable WhenLoaded(Action action)
    {
        action();
        return new Disposable();
    }

    public IDisposable WhenLoadedOnce(Action action)
    {
        action();
        return new Disposable();
    }

    public async Task<IDisposable> WhenLoadedAsync(Func<Task> action)
    {
        await action();
        return new Disposable();
    }

    public async Task<IDisposable> WhenLoadedOnceAsync(Func<Task> action)
    {
        await action();
        return new Disposable();
    }

    public Task Load()
    {
        return Task.CompletedTask;
    }

    public void Reset()
    {
    }

    public void AddRange(IEnumerable<TValue> values)
    {
        foreach (var entityObject in values)
        {
            Add(entityObject);
        }
    }

    public bool NeedsLoading
    {
        get
        {
            return false;
        }
    }

    public Task LoadWith(Task<ApiResult<TValue[]>> promise)
    {
        return Task.CompletedTask;
    }

    public IEnumerable<EntityState<TValue>> GetStates()
    {
        return this.Select(f => new EntityState<TValue>(f, EntityListState.Added));
    }

    public void RemoveWhere(Func<TValue, bool> condition)
    {
        foreach (var entityObject in Items.ToArray())
        {
            if (condition(entityObject))
            {
                Remove(entityObject);
            }
        }
    }

    public Task<TValue> Find(Guid id)
    {
        return Task.FromResult(this.Items.FirstOrDefault(e => e.GetModelIdentifier() == id));
    }

    public EntityState<TValue> State(TValue value)
    {
        return new EntityState<TValue>(value, EntityListState.Added);
    }
}