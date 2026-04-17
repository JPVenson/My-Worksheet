using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MyWorksheet.Shared.Services.PriorityQueue;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater.ArgumentStore;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater.Contracts;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using MyWorksheet.Website.Shared.Services.Activation;
using Newtonsoft.Json;
using ServiceLocator.Attributes;
using IComparable = System.IComparable;

namespace MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater;

public interface IPriorityQueueManager
{
    void LoadFromAttribute(Assembly assemblyToLoadFrom);
    Task<Guid> Enqueue(IComparable priotity, string actionKey, Guid userId, IDictionary<string, object> arguments,
        params string[] requiredCapabilites);
    Task<Guid> Enqueue(PriorityQueueElement priorityElement);
    IDictionary<string, IPriorityQueueAction> PriorityQueueActions { get; }
    IDictionary<IComparable, QueueDispatcher> PriorityQueues { get; }

    Guid SerializeQueueGraph(IPriorityQueueArgumentStore priorityQueueArgumentStore,
        PriorityQueueElement priorityElement);

    Task<bool> DispatchHere(Guid queueId);
}

[SingletonService(typeof(IPriorityQueueManager), typeof(IServerPriorityQueueManager))]
public class PriorityQueueManager : RequireInit, IDisposable, IPriorityQueueManager, IServerPriorityQueueManager
{
    private readonly PriorityQueueOptions _queueOptions;
    private readonly IAppMetricsLogger _client;
    private readonly ILogger<PriorityQueueManager> _logger;
    private readonly IPriorityQueueArgumentStore _priorityQueueArgumentStore;
    private readonly ActivatorService _activatorService;

    public PriorityQueueManager(PriorityQueueOptions queueOptions,
        IAppMetricsLogger appMetricsLogger,
        ILogger<PriorityQueueManager> logger,
        IPriorityQueueArgumentStore priorityQueueArgumentStore,
        ActivatorService activatorService)
    {
        _queueOptions = queueOptions;
        _client = appMetricsLogger;
        PriorityQueueActions = new ConcurrentDictionary<string, IPriorityQueueAction>();
        PriorityQueues = new Dictionary<IComparable, QueueDispatcher>();

        _logger = logger;
        _priorityQueueArgumentStore = priorityQueueArgumentStore;
        _activatorService = activatorService;

        //CreateQueue(IComparable, Environment.ProcessorCount);
        //CreateQueue(PriorityManagerLevel.Realtime, Environment.ProcessorCount / 2);
        //CreateQueue(PriorityManagerLevel.Later, Environment.ProcessorCount / 4, ThreadPriority.BelowNormal);
        //CreateQueue(PriorityManagerLevel.FireAndForget, Environment.ProcessorCount / 8, ThreadPriority.Lowest);
    }

    private void CreateQueue(IComparable level, int count, ThreadPriority priority = ThreadPriority.Normal)
    {
        if (count <= 0)
        {
            count = 1;
        }

        var factorys = new List<ActionDispatcher>();
        for (int i = 0; i < count; i++)
        {
            var factory = new ActionDispatcher(false, "PriorityQueue." + level + "." + i);
            factory.TaskFailedAsync += (exception, trace) =>
            {
                _logger.LogWarning("QueueTask Failed", _queueOptions.LoggerCategory, new Dictionary<string, string>()
                {
                    {
                        "TaskLevel", level.ToString()
                    },
                    {
                        "Count", factorys.Count.ToString()
                    },
                    {
                        "Exeption", JsonConvert.SerializeObject(exception)
                    }
                });
            };
            factory.Priority = priority;
            factory.Timeout = TimeSpan.FromMinutes(1);
            factorys.Add(factory);
        }
        PriorityQueues.Add(level.ToString(), new QueueDispatcher(factorys, level, () => _client, PriorityQueueActions, _queueOptions.Filter,
            _priorityQueueArgumentStore));
        _logger.LogInformation("Created Task Queue", _queueOptions.LoggerCategory, new Dictionary<string, string>()
        {
            {
                "Level", level.ToString()
            },
            {
                "Count", factorys.Count.ToString()
            }
        });
    }

    public override async ValueTask InitAsync(IServiceProvider services)
    {
        foreach (var queueSize in _queueOptions.QueueSize)
        {
            CreateQueue(queueSize.Key, queueSize.Value.NoOfQueues, queueSize.Value.Priority);
        }
        LoadFromAttribute(GetType().Assembly);

        var resume = await _priorityQueueArgumentStore.GetResume();
        foreach (var item in resume)
        {
            PriorityQueues[item.Item2.Level].Dispatch(new TaskEnqueueElement()
            {
                QueueItemId = item.Item1
            });
        }

        await base.InitAsync(services);
    }

    public void LoadFromAttribute(Assembly assemblyToLoadFrom)
    {
        var priorityItems = assemblyToLoadFrom.GetTypes()
            .Where(f => !f.IsInterface && typeof(IPriorityQueueAction).IsAssignableFrom(f))
            .Where(e => e.GetCustomAttribute(typeof(PriorityQueueItemAttribute)) != null)
            .ToDictionary(e => e.GetCustomAttribute<PriorityQueueItemAttribute>(), f => f);

        foreach (var priorityItem in priorityItems)
        {
            PriorityQueueActions.Add(priorityItem.Key.Key, (IPriorityQueueAction)_activatorService.ActivateType(priorityItem.Value));
        }
    }

    public IDictionary<string, IPriorityQueueAction> PriorityQueueActions { get; private set; }
    public IDictionary<IComparable, QueueDispatcher> PriorityQueues { get; private set; }

    public Task<Guid> Enqueue(IComparable priotity, string actionKey, Guid userId,
        IDictionary<string, object> arguments, params string[] requiredCapabilites)
    {
        return Enqueue(new PriorityQueueElement(priotity, userId, actionKey, arguments, new PriorityQueueElement[0]));
    }

    public Task<Guid> Enqueue(PriorityQueueElement priorityElement)
    {
        var inStore = SerializeQueueGraph(_priorityQueueArgumentStore, priorityElement);
        PriorityQueues[priorityElement.Level.ToString()].Dispatch(new TaskEnqueueElement()
        {
            QueueItemId = inStore
        });
        return Task.FromResult(inStore);
    }

    public Guid SerializeQueueGraph(IPriorityQueueArgumentStore priorityQueueArgumentStore, PriorityQueueElement priorityElement)
    {
        var inStore = priorityQueueArgumentStore.SetInStore(priorityElement);
        foreach (var priorityItemFollowUp in priorityElement.FollowUps)
        {
            priorityQueueArgumentStore.SetInStore(priorityItemFollowUp, inStore);
        }

        return inStore;
    }

    public async Task<bool> DispatchHere(Guid queueId)
    {
        var priorityQueueElement = await _priorityQueueArgumentStore.GetFromStore(queueId);
        var queue = PriorityQueues.First(e => e.Key.ToString().Equals(priorityQueueElement.Level.ToString()))
            .Value;
        if (queue != null)
        {
            queue.Dispatch(new TaskEnqueueElement()
            {
                QueueItemId = queueId
            });
            return true;
        }

        return false;
    }

    public void Dispose()
    {
        foreach (var queueDispatcher in PriorityQueues)
        {
            queueDispatcher.Value.Dispose();
        }
    }

    public ProcessorCapability[] ReportCapabilities()
    {
        return PriorityQueueActions.Keys
            .Select(e => new ProcessorCapability()
            {
                Name = e,
                Value = "",
                IsEnabled = true
            })
            .ToArray();
    }
}