using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Katana.CommonTasks.Services.Logging.Contracts;
using MyWorksheet.Website.Server.Services.ExecuteLater.ExecuteLater.ArgumentStore;
using MyWorksheet.Website.Server.Services.ExecuteLater.ExecuteLater.Contracts;
using Newtonsoft.Json;
using IComparable = System.IComparable;

namespace MyWorksheet.Website.Server.Services.ExecuteLater.ExecuteLater
{
	public class PriorityQueueManager : IDisposable, IPriorityQueueManager
	{
		private readonly PriorityQueueOptions _queueOptions;
		private readonly IAppMetricsLogger _appMetricsLogger;
		private readonly IAppLogger _appLogger;
		private readonly IPriorityQueueArgumentStore _priorityQueueArgumentStore;

		public PriorityQueueManager(PriorityQueueOptions queueOptions, IAppMetricsLogger appMetricsLogger, IAppLogger appLogger, IPriorityQueueArgumentStore priorityQueueArgumentStore)
		{
			_queueOptions = queueOptions;
			_appMetricsLogger = appMetricsLogger;
			_appLogger = appLogger;
			_priorityQueueArgumentStore = priorityQueueArgumentStore;
			PriorityQueueActions = new ConcurrentDictionary<string, IPriorityQueueAction>();
			PriorityQueues = new Dictionary<IComparable, QueueDispatcher>();
			_client = _appMetricsLogger;
			foreach (var queueSize in _queueOptions.QueueSize)
			{
				CreateQueue(queueSize.Key, queueSize.Value.NoOfQueues, queueSize.Value.Priority);
			}

			//CreateQueue(IComparable, Environment.ProcessorCount);
			//CreateQueue(PriorityManagerLevel.Realtime, Environment.ProcessorCount / 2);
			//CreateQueue(PriorityManagerLevel.Later, Environment.ProcessorCount / 4, ThreadPriority.BelowNormal);
			//CreateQueue(PriorityManagerLevel.FireAndForget, Environment.ProcessorCount / 8, ThreadPriority.Lowest);
		}

		private IAppMetricsLogger _client;

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
					_appLogger.LogWarning("QueueTask Failed", _queueOptions.LoggerCategory, new Dictionary<string, string>()
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
			PriorityQueues.Add(level, new QueueDispatcher(factorys, level, () => _client,PriorityQueueActions, _queueOptions.Filter));
			_appLogger.LogInformation("Created Task Queue",  _queueOptions.LoggerCategory, new Dictionary<string, string>()
			{
				{
					"Level", level.ToString()
				},
				{
					"Count", factorys.Count.ToString()
				}
			});
		}

		public void LoadFromAttribute(Assembly assemblyToLoadFrom)
		{
			var priorityItems = assemblyToLoadFrom.GetTypes()
				.Where(f => !f.IsInterface && typeof(IPriorityQueueAction).IsAssignableFrom(f))
				.Where(e => e.GetCustomAttribute(typeof(PriorityQueueItemAttribute)) != null)
				.ToDictionary(e => e.GetCustomAttribute<PriorityQueueItemAttribute>(), f => f);
			
			foreach (var priorityItem in priorityItems)
			{
				PriorityQueueActions.Add(priorityItem.Key.Key, (IPriorityQueueAction)Activator.CreateInstance(priorityItem.Value));
			}
		}
		
		public IDictionary<string, IPriorityQueueAction> PriorityQueueActions { get; private set; }
		public IDictionary<IComparable, QueueDispatcher> PriorityQueues { get; private set; }

		public Task<int> Enqueue(IComparable priotity, string actionKey, Guid userId,
			IDictionary<string, object> arguments, params string[] requiredCapabilites)
		{
			return Enqueue(new PriorityQueueElement(priotity, userId, actionKey, arguments, new PriorityQueueElement[0]));
		}

		public Task<int> Enqueue(PriorityQueueElement priorityElement)
		{
			int inStore;
			PriorityQueues[priorityElement.Level].Dispatch(new TaskEnqueueElement()
			{
				QueueItemId = inStore = SerializeQueueGraph(_priorityQueueArgumentStore, priorityElement)
			});
			return Task.FromResult(inStore);
		}

		public int SerializeQueueGraph(IPriorityQueueArgumentStore priorityQueueArgumentStore, PriorityQueueElement priorityElement)
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
	}
}