using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels;

namespace MyWorksheet.Website.Client.Util.Promise;

public class FutureList<TValue> : IFutureList<TValue>
{
    protected readonly Func<Task<ApiResult<TValue[]>>> Loader;
    private Collection<TValue> _items;

    public static Func<Task<ApiResult<TValue[]>>> Empty = () => Task.FromResult(new ApiResult<TValue[]>(HttpStatusCode.OK, true, new TValue[0], "OK"));

    public FutureList(Func<Task<ApiResult<TValue[]>>> loader)
    {
        Loader = loader;
        _items = new Collection<TValue>();
    }

    public FutureList(Func<Task<TValue[]>> loader)
    {
        Loader = async () => new ApiResult<TValue[]>(HttpStatusCode.OK, true, (await loader()), null);
        _items = new Collection<TValue>();
    }

    public event EventHandler ListLoaded;

    public virtual void Reset()
    {
        _items.Clear();
        Loaded = false;
    }

    public bool Loaded { get; protected set; }
    public bool Loading { get; protected set; }

    public bool NeedsLoading
    {
        get
        {
            return !Loaded && !Loading;
        }
    }

    public virtual void AddRange(IEnumerable<TValue> values)
    {
        foreach (var value in values)
        {
            _items.Add(value);
        }
    }

    private Task<ApiResult<TValue[]>> _loaderTask;

    public virtual Task LoadWith(Task<ApiResult<TValue[]>> promise)
    {
        Loading = true;
        return _loaderTask = promise.ContinueWith(t =>
        {
            Loading = false;
            Loaded = true;
            if (!t.Result.Success)
            {
                return t.Result;
            }

            AddRange(t.Result.Object ?? Enumerable.Empty<TValue>());
            OnListLoaded();
            return t.Result;
        });
    }

    protected virtual void OnListLoaded()
    {
        ListLoaded?.Invoke(this, EventArgs.Empty);
    }

    private class AsyncEnumerator : IAsyncEnumerator<TValue>
    {
        private readonly Func<Task<ApiResult<TValue[]>>> _loaderTask;
        private readonly IEnumerator<TValue> _source;

        public AsyncEnumerator(Func<Task<ApiResult<TValue[]>>> loaderTask, IFutureList<TValue> source)
        {
            _loaderTask = loaderTask;
            _source = source.GetEnumerator();
        }

        public async ValueTask DisposeAsync()
        {
            _source.Dispose();
        }

        public async ValueTask<bool> MoveNextAsync()
        {
            var loaderTask = _loaderTask();
            await loaderTask;
            return _source.MoveNext();
        }

        public TValue Current
        {
            get
            {
                return _source.Current;
            }
        }
    }

    public IAsyncEnumerator<TValue> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken())
    {
        return new AsyncEnumerator(() => _loaderTask, this);
    }

    public virtual IEnumerator<TValue> GetEnumerator()
    {
        Load();
        return _items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_items).GetEnumerator();
    }

    public virtual IDisposable WhenLoaded(Action action)
    {
        void Invoke(object? sender, EventArgs eventArgs)
        {
            action();
        }

        ListLoaded += Invoke;
        if (Loaded)
        {
            action();
        }

        return new Disposable(() =>
        {
            ListLoaded -= Invoke;
        });
    }

    public virtual IDisposable WhenLoadedOnce(Action action)
    {
        if (Loaded)
        {
            action();
            return new Disposable(() => { });
        }

        void Invoke(object? sender, EventArgs eventArgs)
        {
            action();
            ListLoaded -= Invoke;
        }

        ListLoaded += Invoke;

        return new Disposable(() =>
        {
            ListLoaded -= Invoke;
        });
    }

    public virtual async Task<IDisposable> WhenLoadedAsync(Func<Task> action)
    {
        async void Invoke(object? sender, EventArgs eventArgs)
        {
            await action();
        }

        ListLoaded += Invoke;
        if (Loaded)
        {
            await action();
        }

        return new Disposable(() =>
        {
            ListLoaded -= Invoke;
        });
    }

    public virtual async Task<IDisposable> WhenLoadedOnceAsync(Func<Task> action)
    {
        if (Loaded)
        {
            await action();
            return new Disposable(() => { });
        }

        async void Invoke(object? sender, EventArgs eventArgs)
        {
            ListLoaded -= Invoke;
            await action();
        }

        ListLoaded += Invoke;

        return new Disposable(() =>
        {
            ListLoaded -= Invoke;
        });
    }

    public virtual Task Load()
    {
        if (NeedsLoading)
        {
            return LoadWith(Loader());
        }
        return Task.CompletedTask;
    }

    public virtual void RemoveWhere(Func<TValue, bool> condition)
    {
        foreach (var value in _items.ToArray())
        {
            if (condition(value))
            {
                _items.Remove(value);
            }
        }
    }

    public async Task<TValue> Find(Guid id)
    {
        await Load();
        return _items.FirstOrDefault(e => e.GetId() == id);
    }

    public virtual void Add(TValue item)
    {
        _items.Add(item);
    }

    public virtual void Clear()
    {
        _items.Clear();
    }

    public virtual bool Contains(TValue item)
    {
        return _items.Contains(item);
    }

    public virtual void CopyTo(TValue[] array, int arrayIndex)
    {
        _items.CopyTo(array, arrayIndex);
    }

    public virtual bool Remove(TValue item)
    {
        return _items.Remove(item);
    }

    public virtual int Count
    {
        get
        {
            Load();
            return _items.Count;
        }
    }

    public virtual bool IsReadOnly
    {
        get { return NeedsLoading; }
    }

    public virtual int IndexOf(TValue item)
    {
        Load();
        return _items.IndexOf(item);
    }

    public virtual void Insert(int index, TValue item)
    {
        Load();
        _items.Insert(index, item);
    }

    public virtual void RemoveAt(int index)
    {
        Load();
        _items.RemoveAt(index);
    }

    public virtual TValue this[int index]
    {
        get
        {
            Load();
            return _items[index];
        }
        set
        {
            _items[index] = value;
        }
    }
}