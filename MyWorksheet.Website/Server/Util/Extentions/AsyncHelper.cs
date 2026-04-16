using System;
using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace MyWorksheet.Shared.Helper;

/// <summary>
/// A Helper class to run Asynchronous functions from synchronous ones
/// </summary>
public static class AsyncHelper
{
    public static Func<object> WrapAsFunc(this Action action)
    {
        return () =>
        {
            action();
            return null;
        };
    }

    public static Func<Task> WrapAsAsync(this Action action)
    {
        return async () =>
        {
            action();
            await Task.CompletedTask;
        };
    }

    public static Task<bool> WaitOneAsync(this WaitHandle waitHandle)
    {
        return WaitOneAsync(waitHandle, TimeSpan.Zero);
    }
    public static Task<bool> WaitOneAsync(this WaitHandle waitHandle, TimeSpan timeoutMs)
    {
        if (waitHandle == null)
        {
            throw new ArgumentNullException(nameof(waitHandle));
        }

        var tcs = new TaskCompletionSource<bool>();

        var registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(
            waitHandle,
            (state, timedOut) => { tcs.TrySetResult(!timedOut); },
            null,
            timeoutMs,
            true);

        return tcs.Task.ContinueWith((antecedent) =>
        {
            registeredWaitHandle.Unregister(waitObject: null);
            try
            {
                return antecedent.Result;
            }
            catch
            {
                return false;
                throw;
            }
        });
    }
    /// <summary>
    /// A class to bridge synchronous asynchronous methods
    /// </summary>
    public class AsyncBridge : IDisposable
    {
        private readonly bool _expectTask;
        private readonly ExclusiveSynchronizationContext _currentContext;
        private readonly SynchronizationContext _oldContext;
        private int _taskCount;

        /// <summary>
        /// Constructs the AsyncBridge by capturing the current
        /// SynchronizationContext and replacing it with a new
        /// ExclusiveSynchronizationContext.
        /// </summary>
        internal AsyncBridge(bool expectTask)
        {
            _expectTask = expectTask;
            _oldContext = SynchronizationContext.Current;
            _currentContext =
                new ExclusiveSynchronizationContext(_oldContext, expectTask);
            SynchronizationContext
                .SetSynchronizationContext(_currentContext);
        }

        /// <summary>
        /// Execute's an async task with a void return type
        /// from a synchronous context
        /// </summary>
        /// <param name="task">Task to execute</param>
        /// <param name="callback">Optional callback</param>
        public void Run(Task task, Action<Task> callback = null)
        {
            _currentContext.Post(async _ =>
            {
                try
                {
                    Increment();
                    await task;

                    callback?.Invoke(task);
                }
                catch (Exception e)
                {
                    _currentContext.InnerException = ExceptionDispatchInfo.Capture(e);
                }
                finally
                {
                    Decrement();
                }
            }, null);
        }

        /// <summary>
        /// Execute's an async task with a T return type
        /// from a synchronous context
        /// </summary>
        /// <typeparam name="T">The type of the task</typeparam>
        /// <param name="task">Task to execute</param>
        /// <param name="callback">Optional callback</param>
        public void Run<T>(Task<T> task, Action<Task<T>> callback = null)
        {
            if (null != callback)
            {
                Run((Task)task, (finishedTask) =>
                    callback((Task<T>)finishedTask));
            }
            else
            {
                Run((Task)task);
            }
        }

        /// <summary>
        /// Execute's an async task with a T return type
        /// from a synchronous context
        /// </summary>
        /// <typeparam name="T">The type of the task</typeparam>
        /// <param name="task">Task to execute</param>
        /// <param name="callback">
        /// The callback function that uses the result of the task
        /// </param>
        public void Run<T>(Task<T> task, Action<T> callback)
        {
            Run(task, (t) => callback(t.Result));
        }

        private void Increment()
        {
            Interlocked.Increment(ref _taskCount);
        }

        private void Decrement()
        {
            Interlocked.Decrement(ref _taskCount);
            if (_taskCount == 0)
            {
                _currentContext.EndMessageLoop();
            }
        }

        /// <summary>
        /// Disposes the object
        /// </summary>
        public void Dispose()
        {
            if (_taskCount == 0 && !_expectTask)
            {
                return;
            }
            try
            {
                _currentContext.BeginMessageLoop();
            }
            finally
            {
                SynchronizationContext
                    .SetSynchronizationContext(_oldContext);
            }
        }
    }

    /// <summary>
    /// Creates a new AsyncBridge. This should always be used in
    /// conjunction with the using statement, to ensure it is disposed
    /// </summary>
    public static AsyncBridge Wait
    {
        get { return new AsyncBridge(false); }
    }


    /// <summary>
    /// Awaits a single Task
    /// </summary>
    /// <param name="task">The task.</param>
    public static void WaitSingle(Task task)
    {
        using (var bridge = new AsyncBridge(true))
        {
            bridge.Run(task);
        }
    }

    /// <summary>
    /// Awaits a single Task
    /// </summary>
    /// <param name="task">The task.</param>
    public static TResult WaitSingle<TResult>(Task<TResult> task)
    {
        using (var bridge = new AsyncBridge(true))
        {
            bridge.Run(task);
        }

        return task.Result;
    }

    /// <summary>
    /// Runs a task with the "Fire and Forget" pattern using Task.Run,
    /// and unwraps and handles exceptions
    /// </summary>
    /// <param name="task">A function that returns the task to run</param>
    /// <param name="handle">Error handling action, null by default</param>
    public static void FireAndForget(
        Func<Task> task,
        Action<Exception> handle = null)
    {
        Task.Run(
            () =>
            {
                ((Func<Task>)(async () =>
                {
                    try
                    {
                        await task();
                    }
                    catch (Exception e)
                    {
                        handle?.Invoke(e);
                    }
                }))();
            });
    }

    private class ExclusiveSynchronizationContext : SynchronizationContext
    {
        private readonly bool _expectTask;

        private readonly AutoResetEvent _workItemsWaiting =
            new AutoResetEvent(false);

        private bool _done;
        private readonly ConcurrentQueue<Tuple<SendOrPostCallback, object>> _items;

        public ExceptionDispatchInfo InnerException { get; set; }

        public ExclusiveSynchronizationContext(SynchronizationContext old, bool expectTask)
        {
            _expectTask = expectTask;
            var oldEx =
                old as ExclusiveSynchronizationContext;

            if (null != oldEx)
            {
                this._items = oldEx._items;
            }
            else
            {
                this._items = new ConcurrentQueue<Tuple<SendOrPostCallback, object>>();
            }
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            throw new NotSupportedException(
                "We cannot send to our same thread");
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            _items.Enqueue(Tuple.Create(d, state));
            _workItemsWaiting.Set();
        }

        public void EndMessageLoop()
        {
            Post(_ => _done = true, null);
        }

        public void BeginMessageLoop()
        {
            while (!_done)
            {
                Tuple<SendOrPostCallback, object> task = null;

                if (!_items.TryDequeue(out task))
                {
                    task = null;
                }

                if (task != null)
                {
                    task.Item1(task.Item2);
                    if (InnerException != null) // method threw an exeption
                    {
                        InnerException.Throw();
                    }
                }
                else
                {
                    _workItemsWaiting.WaitOne();
                }
            }
        }

        public override SynchronizationContext CreateCopy()
        {
            return this;
        }
    }
}