// Erstellt von Jean-Pierre Bachmann am 11:06

using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MyWorksheet.Shared.Helper;

namespace MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater;

/// <summary>
///		Creates a Queue of Actions that will be called asyncrolly as they are added
/// </summary>
public class ActionDispatcher : SerialTaskDispatcherBase, IDisposable
{
    /// <inheritdoc />
    public ActionDispatcher(bool keepRunning = false, [CallerMemberName] string namedConsumer = null) : base(keepRunning)
    {
        _namedConsumer = namedConsumer;
        ConcurrentQueue = new ConcurrentQueue<Func<Task>>();
        Timeout = DefaultTimeout;
    }
    /// <summary>
    /// Current queued Actions
    /// </summary>
    public ConcurrentQueue<Func<Task>> ConcurrentQueue { get; private set; }

    /// <summary>
    /// Adds an Action to the Queue and starts the scheduler
    /// </summary>
    /// <param name="action"></param>
    public void Add(Action action)
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException("The Instance of the " + nameof(ActionDispatcher) + " was disposed and cannot accept new Actions");
        }

        TryAdd(action);
    }

    /// <summary>
    /// Adds an Action to the Queue and starts the scheduler
    /// </summary>
    /// <param name="action"></param>
    public bool TryAdd(Action action)
    {
        if (_isDisposed)
        {
            return false;
        }

        if (Thread.CurrentThread == _thread)
        {
            action();
            return true;
        }

        ConcurrentQueue.Enqueue(action.WrapAsAsync());
        StartScheduler();
        return true;
    }


    /// <summary>
    /// Adds an Action to the Queue and starts the scheduler
    /// </summary>
    /// <param name="action"></param>
    public void Add(Func<Task> action)
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException("The Instance of the " + nameof(ActionDispatcher) + " was disposed and cannot accept new Actions");
        }

        TryAdd(action);
    }

    /// <summary>
    /// Adds an Action to the Queue and starts the scheduler
    /// </summary>
    /// <param name="action"></param>
    public bool TryAdd(Func<Task> action)
    {
        if (_isDisposed)
        {
            return false;
        }

        if (Thread.CurrentThread == _thread)
        {
            action().GetAwaiter().GetResult();
            return true;
        }

        ConcurrentQueue.Enqueue(action);
        StartScheduler();
        return true;
    }

    protected override Func<Task> GetNext()
    {
        ConcurrentQueue.TryDequeue(out var next);
        return next;
    }

    protected override bool HasNext()
    {
        return !ConcurrentQueue.IsEmpty;
    }
}