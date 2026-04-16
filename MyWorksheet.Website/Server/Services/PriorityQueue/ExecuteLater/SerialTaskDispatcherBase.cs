using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater;

public static class NoSynchronizationContextScope
{
    public static Disposable Enter()
    {
        var context = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(null);
        return new Disposable(context);
    }

    public struct Disposable : IDisposable
    {
        private readonly SynchronizationContext _synchronizationContext;

        public Disposable(SynchronizationContext synchronizationContext)
        {
            _synchronizationContext = synchronizationContext;
        }

        public void Dispose() =>
            SynchronizationContext.SetSynchronizationContext(_synchronizationContext);
    }
}

/// <summary>
///		Base class for the Serial Factory base
/// </summary>
public abstract class SerialTaskDispatcherBase : IDisposable
{
    private readonly bool _keepRunning;

    /// <summary>
    /// Initializes a new instance of the <see cref="SerialTaskDispatcherBase"/> class.
    /// </summary>
    /// <param name="keepRunning">if set to <c>true</c> the background thread will keep running until disposed.</param>
    public SerialTaskDispatcherBase(bool keepRunning)
    {
        _keepRunning = keepRunning;
    }

    protected internal string _namedConsumer;
    protected internal Thread _thread;
    protected internal readonly object _lockRoot = new object();
    protected internal bool _isDisposed;

    /// <summary>
    /// Default Timeout for disposing
    /// </summary>
    public static readonly TimeSpan DefaultTimeout = new TimeSpan(0, 0, 0, 5);

    /// <summary>
    /// Timeout for disposing
    /// </summary>
    public TimeSpan Timeout { get; set; }

    protected internal volatile bool _isWorking;

    protected internal EventHandler GetStateChanged()
    {
        return StateChanged;
    }

    protected internal EventHandler<Exception> GetTaskFailedAsync()
    {
        return TaskFailedAsync;
    }

    protected internal void ResetStateChanged()
    {
        StateChanged = null;
    }

    protected internal void ResetTaskFailedAsync()
    {
        TaskFailedAsync = null;
    }

    /// <summary>
    /// Will be invoked when the Status of the Current Thread has Changed
    /// </summary>
    public event EventHandler StateChanged;

    /// <summary>
    /// Will be invoked when any task has Failed
    /// </summary>
    public event EventHandler<Exception> TaskFailedAsync;

    /// <summary>
    /// Returns true if the Current Thread is working on any item inside the ConcurrentQueue
    /// </summary>
    // ReSharper disable once InconsistentlySynchronizedField
    public bool IsWorking => _isWorking;

    protected abstract Func<Task> GetNext();
    protected abstract bool HasNext();

    internal async Task Worker()
    {
        try
        {
            Func<Task> action;
            while ((action = GetNext()) != null && !_isDisposed)
            {
                try
                {
                    await action();
                    //if (invoke is Task task)
                    //{
                    //	await task;
                    //	//AsyncHelper.WaitSingle(task);
                    //	//task.ConfigureAwait(false).GetAwaiter().GetResult();
                    //}
                }
                catch (Exception e)
                {
                    OnTaskFailed(e);
                }
            }
        }
        finally
        {
            lock (_lockRoot)
            {
                _isWorking = false;
                OnStateChanged();
                //in case that while we were executing the cleanup, another task was added
                if (HasNext() && !_isDisposed)
                {
                    StartScheduler();
                }
            }
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _isDisposed = true;
        if (_thread != null)
        {
            //give the thread time to complete
            _thread.Join(Timeout);
            if (_thread.IsAlive)
            {
                //Thread seems to be waiting for a resource so interrupt all sleeps or wait handels
                _thread.Interrupt();
                //the Thread should have been listening!!
                _thread.Abort();
                if (_thread.IsAlive)
                {
                    //await any cleanup or finalizer to be run
                    _thread.Join(Timeout);
                }
            }
            _thread = null;
        }

        _isWorking = false;
        ResetStateChanged();
        ResetTaskFailedAsync();
    }

    /// <summary>
    /// Raises the StateChanged Event
    /// </summary>
    protected virtual void OnStateChanged()
    {
        GetStateChanged()?.Invoke(this, EventArgs.Empty);
    }
    /// <summary>
    /// Raises the TaskFailedAsync Event
    /// </summary>
    protected virtual void OnTaskFailed(Exception e)
    {
        var taskFailedEvent = GetTaskFailedAsync();
        if (taskFailedEvent != null)
        {
            Task.Run(() => taskFailedEvent.Invoke(this, e));
        }
    }

    readonly AutoResetEvent _continiusWorker = new AutoResetEvent(false);

    public ThreadPriority Priority { get; set; }

    protected internal void StartScheduler()
    {
        lock (_lockRoot)
        {
            _continiusWorker.Set();
            if (_isWorking)
            {
                return;
            }

            _isWorking = true;

            //HostingEnvironment.QueueBackgroundWorkItem(async (d) =>
            //{
            //	await Task.Run(async () =>
            //	{
            //		await Task.Delay(1000);
            //	});
            //	Console.WriteLine();
            //});

            ThreadPool.QueueUserWorkItem(
                async (token) =>
                {
                    if (!_keepRunning)
                    {
                        await Worker();
                    }
                    else
                    {
                        while (_continiusWorker.WaitOne())
                        {
                            await Worker();
                        }
                    }
                }
            );

            //HostingEnvironment.QueueBackgroundWorkItem(async (token) =>
            //{
            //	if (!_keepRunning)
            //	{
            //		await Worker();
            //	}
            //	else
            //	{
            //		while (_continiusWorker.WaitOne())
            //		{
            //			await Worker();
            //		}
            //	}
            //});

            //_thread = new Thread(() =>
            //{
            //	if (!_keepRunning)
            //	{
            //		Worker();
            //	}
            //	else
            //	{
            //		while (_continiusWorker.WaitOne())
            //		{
            //			Worker();
            //		}
            //	}
            //});
            //_thread.Name = _namedConsumer;
            //_thread.Priority = Priority;
            //_thread.SetApartmentState(ApartmentState.STA);
            //_thread.Start();
            OnStateChanged();
        }
    }
}