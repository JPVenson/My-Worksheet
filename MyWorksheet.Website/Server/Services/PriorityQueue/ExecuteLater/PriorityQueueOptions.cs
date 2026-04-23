using System;
using System.Collections.Generic;
using System.Threading;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater.ArgumentStore;
using MyWorksheet.Website.Server.Shared.Services.Logger;
using MyWorksheet.Website.Shared.Services;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater;

[SingletonService(typeof(IRequireInit))]
public class PriorityQueueOptions : RequireInit
{
    public PriorityQueueOptions() : this("ServerQueue", f => true)
    {
        QueueSize = new Dictionary<IComparable, QueueCreationOptions>();
        Order = 4600;
    }

    public PriorityQueueOptions(string loggerCategory, Func<PriorityQueueElement, bool> filter)
    {
        LoggerCategory = loggerCategory;
        Filter = filter;
    }

    public string LoggerCategory { get; private set; }
    public IDictionary<IComparable, QueueCreationOptions> QueueSize { get; private set; }

    public Func<PriorityQueueElement, bool> Filter { get; private set; }

    public override void Init(IServiceProvider services)
    {
        LoggerCategory = LoggerCategories.Server.ToString();
        QueueSize.Add(PriorityManagerLevel.System, new QueueCreationOptions(1));
        QueueSize.Add(PriorityManagerLevel.Realtime, new QueueCreationOptions(1));
        QueueSize.Add(PriorityManagerLevel.Later, new QueueCreationOptions(1, ThreadPriority.BelowNormal));
        QueueSize.Add(PriorityManagerLevel.FireAndForget, new QueueCreationOptions(1, ThreadPriority.Lowest));
    }
}