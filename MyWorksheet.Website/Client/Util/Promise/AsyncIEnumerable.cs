using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MyWorksheet.Website.Client.Util.Promise;

public class AsyncIEnumerable<TValue> : IAsyncIEnumerable<TValue>
{
    private readonly Task<IEnumerable<TValue>> _promise;
    private IEnumerable<TValue> _value;

    public AsyncIEnumerable(Task<IEnumerable<TValue>> promise)
    {
        _promise = promise;
        _value = Enumerable.Empty<TValue>();
    }

    public AsyncIEnumerable(IEnumerable<TValue> values)
    {
        _value = values;
    }

    public void WhenAvailable(Action action)
    {
        if (_promise == null || _promise.IsCompleted)
        {
            action();
        }
        else
        {
            _promise.ContinueWith((t) => action());
        }
    }

    public IEnumerator<TValue> GetEnumerator()
    {
        return _value.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}