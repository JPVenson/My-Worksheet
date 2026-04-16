using System.Threading;

namespace MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater;

public class QueueCreationOptions
{
    public QueueCreationOptions(int noOfQueues, ThreadPriority priority = ThreadPriority.Normal)
    {
        NoOfQueues = noOfQueues;
        Priority = priority;
    }

    public int NoOfQueues { get; private set; }
    public ThreadPriority Priority { get; }
}