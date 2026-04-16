using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MyWorksheet.Webpage.Helper.Utlitiys;

public class ConcurrentHashSet<T> : IDisposable, IEnumerable<T>
{
    public ConcurrentHashSet()
        : this(EqualityComparer<T>.Default)
    {

    }

    public ConcurrentHashSet(IEqualityComparer<T> equalityComparer)
    {
        _hashSet = new HashSet<T>(equalityComparer);
        _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
    }

    private readonly ReaderWriterLockSlim _lock;
    private readonly HashSet<T> _hashSet;

    public T Replace(T item, T nItem)
    {
        try
        {
            _lock.EnterWriteLock();
            Remove(item);
            TryAdd(nItem);
            return nItem;
        }
        finally
        {
            if (_lock.IsWriteLockHeld)
            {
                _lock.ExitWriteLock();
            }
        }
    }

    public T Get(Func<T, bool> predicate)
    {
        try
        {
            _lock.EnterReadLock(); ;
            return _hashSet.FirstOrDefault(predicate);
        }
        finally
        {
            if (_lock.IsReadLockHeld)
            {
                _lock.ExitReadLock();
            }
        }
    }

    public bool TryAdd(T item)
    {
        try
        {
            _lock.EnterWriteLock();
            return _hashSet.Add(item);
        }
        finally
        {
            if (_lock.IsWriteLockHeld)
            {
                _lock.ExitWriteLock();
            }
        }
    }

    public void Clear()
    {
        try
        {
            _lock.EnterWriteLock();
            _hashSet.Clear();
        }
        finally
        {
            if (_lock.IsWriteLockHeld)
            {
                _lock.ExitWriteLock();
            }
        }
    }

    public bool Contains(T item)
    {
        try
        {
            _lock.EnterReadLock();
            return _hashSet.Contains(item);
        }
        finally
        {
            if (_lock.IsReadLockHeld)
            {
                _lock.ExitReadLock();
            }
        }
    }

    public bool Remove(T item)
    {
        try
        {
            _lock.EnterWriteLock();
            return _hashSet.Remove(item);
        }
        finally
        {
            if (_lock.IsWriteLockHeld)
            {
                _lock.ExitWriteLock();
            }
        }
    }

    public int RemoveWhere(Predicate<T> predicate)
    {
        try
        {
            _lock.EnterWriteLock();
            return _hashSet.RemoveWhere(predicate);
        }
        finally
        {
            if (_lock.IsWriteLockHeld)
            {
                _lock.ExitWriteLock();
            }
        }
    }

    public int Count
    {
        get
        {
            try
            {
                _lock.EnterReadLock();
                return _hashSet.Count;
            }
            finally
            {
                if (_lock.IsReadLockHeld)
                {
                    _lock.ExitReadLock();
                }
            }
        }
    }

    public void Dispose()
    {
        if (_lock != null)
        {
            _lock.Dispose();
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        try
        {
            _lock.EnterReadLock();
            return _hashSet.ToList().GetEnumerator();
        }
        finally
        {
            if (_lock.IsReadLockHeld)
            {
                _lock.ExitReadLock();
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}