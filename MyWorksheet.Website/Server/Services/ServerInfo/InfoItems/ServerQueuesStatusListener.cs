using System;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater.ArgumentStore;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;

namespace MyWorksheet.Website.Server.Services.ServerInfo.InfoItems;

public class ServerQueuesStatusListener : ServerPullInfoListener
{
    readonly IPriorityQueueManager _priorityQueueManager;
    public ServerQueuesStatusListener(Action<string, object> publisher, IPriorityQueueManager priorityQueueManager,
        IAppLogger logger) : base(publisher, logger)
    {
        _priorityQueueManager = priorityQueueManager;
        foreach (var queueDispatcher in _priorityQueueManager.PriorityQueues)
        {
            queueDispatcher.Value.TaskEnqueing += Value_TaskEnqueing;
            queueDispatcher.Value.TaskExecuting += ValueTaskExecuting;
            queueDispatcher.Value.TaskDequeing += ValueOnTaskDequeing;
        }

        Consumers.Add(PublishValue);
    }

    protected override void OnDispose()
    {
        Consumers.Remove(PublishValue);
        base.OnDispose();
    }

    private void ValueOnTaskDequeing(ActionDispatcher arg1, int arg2, PriorityQueueElement arg3, IComparable arg4)
    {
        PublishValue(arg4, arg2, arg1.ConcurrentQueue.Count, arg3);
    }

    private void ValueTaskExecuting(ActionDispatcher arg1, int arg2, PriorityQueueElement arg3, IComparable arg4)
    {
        PublishValue(arg4, arg2, arg1.ConcurrentQueue.Count, arg3);
    }

    private void Value_TaskEnqueing(ActionDispatcher seriellTaskFactory, int i, IComparable arg3)
    {
        PublishValue(arg3, i, seriellTaskFactory.ConcurrentQueue.Count + 1, null);
    }

    public override string Key { get; } = "priority_queue";
    public override void PublishValue()
    {
        foreach (var queueDispatcher in _priorityQueueManager.PriorityQueues)
        {
            for (var index = 0; index < queueDispatcher.Value.Queues.Count; index++)
            {
                var queue = queueDispatcher.Value.Queues[index];
                PublishValue(queueDispatcher.Key, index, queue.ConcurrentQueue.Count, null);
            }
        }
    }

    public void PublishValue(IComparable level, int noOfQueue, int sizeOfQueue, PriorityQueueElement task)
    {
        Publish(new
        {
            Level = level.ToString(),
            No = noOfQueue,
            CurrentTask = task,
            QueueSize = sizeOfQueue
        });
    }
}