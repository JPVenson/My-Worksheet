using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Shared.Services.Logging.Default;

public class CircularBuffer<T> : IProducerConsumerCollection<T>
{
    IProducerConsumerCollection<T> _queue;
    int _size;

    public CircularBuffer(int size)
    {
        _queue = new ConcurrentQueue<T>();
        _size = size;
    }

    public void Add(T obj)
    {
        if (_queue.Count == _size)
        {
            T obj2;
            _queue.TryTake(out obj2);
            _queue.TryAdd(obj);
        }
        else
        {
            _queue.TryAdd(obj);
        }
    }

    public T Peek()
    {
        T obj;
        _queue.TryTake(out obj);
        return obj;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _queue.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_queue).GetEnumerator();
    }

    public void CopyTo(Array array, int index)
    {
        _queue.CopyTo(array, index);
    }

    public int Count => _queue.Count;

    public object SyncRoot => _queue.SyncRoot;

    public bool IsSynchronized => _queue.IsSynchronized;

    public void CopyTo(T[] array, int index)
    {
        _queue.CopyTo(array, index);
    }

    public bool TryAdd(T item)
    {
        return _queue.TryAdd(item);
    }

    public bool TryTake(out T item)
    {
        return _queue.TryTake(out item);
    }

    public T[] ToArray()
    {
        return _queue.ToArray();
    }

    public void Clear()
    {
        while (TryTake(out var item))
        {

        }
    }
}