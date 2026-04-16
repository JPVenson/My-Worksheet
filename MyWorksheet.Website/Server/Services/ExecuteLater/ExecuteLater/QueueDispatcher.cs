#region

using System;
using System.Collections.Generic;
using System.Linq;
using Katana.CommonTasks.Services.Logging.Contracts;
using MyWorksheet.Website.Server.Services.ExecuteLater.ExecuteLater.ArgumentStore;
using MyWorksheet.Website.Server.Services.ExecuteLater.ExecuteLater.Contracts;

#endregion

namespace MyWorksheet.Website.Server.Services.ExecuteLater.ExecuteLater
{
	public class QueueDispatcher : IDisposable
	{
		private readonly Func<IAppMetricsLogger> _client;
		private readonly Func<PriorityQueueElement, bool> _filter;
		private readonly IPriorityQueueArgumentStore _priorityQueueArgumentStore;

		private readonly IComparable _level;

		public QueueDispatcher(IList<ActionDispatcher> queues,
			IComparable level,
			Func<IAppMetricsLogger> client,
			IDictionary<string, IPriorityQueueAction> priorityQueueActions,
			Func<PriorityQueueElement, bool> filter,
			IPriorityQueueArgumentStore priorityQueueArgumentStore)
		{
			Queues = queues;
			_level = level;
			_client = client;
			_filter = filter;
			_priorityQueueArgumentStore = priorityQueueArgumentStore;
			PriorityQueueActions = priorityQueueActions;
		}

		public IList<ActionDispatcher> Queues { get; }
		public IDictionary<string, IPriorityQueueAction> PriorityQueueActions { get; private set; }

		public event Action<ActionDispatcher, int, IComparable> TaskEnqueing;
		public event Action<ActionDispatcher, int, PriorityQueueElement, IComparable> TaskExecuting;
		public event Action<ActionDispatcher, int, PriorityQueueElement, IComparable> TaskDequeing;

		public Tuple<int, ActionDispatcher> Dispatch(TaskEnqueueElement task)
		{
			var hasClient = _client();

			if (hasClient != null)
			{
				for (int i = 0; i < Queues.Count; i++)
				{
					var queue = Queues[i];
					hasClient.TrackMetric("Queue." + _level + "." + i, queue.ConcurrentQueue.Count);
				}
			}

			var seriellTaskFactory = Queues.OrderBy(f => f.ConcurrentQueue.Count).FirstOrDefault();
			if (seriellTaskFactory == null)
			{
				return new Tuple<int, ActionDispatcher>(-1, null);
			}

			var indexOf = Queues.IndexOf(seriellTaskFactory);
			OnTaskEnqueing(seriellTaskFactory, indexOf, _level);
			seriellTaskFactory.Add(async () =>
			{
				PriorityQueueElement priorityQueueElement = null;
				try
				{
					priorityQueueElement = await _priorityQueueArgumentStore.GetFromStore(task.QueueItemId);
					
					if (priorityQueueElement == null)
					{
						return;
					}

					if (!this._filter(priorityQueueElement))
					{
						return;
					}
					
					var priorityQueueAction = PriorityQueueActions[priorityQueueElement.ActionKey];
					OnTaskExecuting(seriellTaskFactory, indexOf, priorityQueueElement, _level);
					await priorityQueueAction
						.Execute(priorityQueueElement);
					_priorityQueueArgumentStore.SetDone(task.QueueItemId);

					foreach (var queueItem in _priorityQueueArgumentStore.GetChilds(task.QueueItemId))
					{
						Dispatch(new TaskEnqueueElement()
						{
							QueueItemId = queueItem
						});
					}
				}
				catch (Exception e)
				{
					_priorityQueueArgumentStore.SetFailed(task.QueueItemId, e);
				}
				finally
				{
					hasClient?.TrackMetric("Queue." + _level + "." + indexOf, seriellTaskFactory.ConcurrentQueue.Count - 1);
					OnTaskDequeing(seriellTaskFactory, indexOf, priorityQueueElement, _level);
				}
			});
			return new Tuple<int, ActionDispatcher>(indexOf, seriellTaskFactory);
		}

		protected virtual void OnTaskEnqueing(ActionDispatcher factory, int indexOfQueue, IComparable level)
		{
			TaskEnqueing?.Invoke(factory, indexOfQueue, level);
		}

		protected virtual void OnTaskDequeing(ActionDispatcher factory, int indexOfQueue,
				PriorityQueueElement queueElement, IComparable level)
		{
			TaskDequeing?.Invoke(factory, indexOfQueue, queueElement, level);
		}

		protected virtual void OnTaskExecuting(ActionDispatcher factory, int indexOfQueue,
				PriorityQueueElement queueElement, IComparable level)
		{
			TaskExecuting?.Invoke(factory, indexOfQueue, queueElement, level);
		}

		public void Dispose()
		{
			foreach (var serielTaskFactory in Queues)
			{
				serielTaskFactory.Dispose();
			}
		}
	}
}