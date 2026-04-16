using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Services.ExecuteLater.ExecuteLater.ArgumentStore;
using MyWorksheet.Website.Server.Services.ExecuteLater.ExecuteLater.Contracts;

namespace MyWorksheet.Website.Server.Services.ExecuteLater.ExecuteLater
{
	public interface IPriorityQueueManager
	{
		void LoadFromAttribute(Assembly assemblyToLoadFrom);
		Task<int> Enqueue(IComparable priotity, string actionKey, Guid userId, IDictionary<string, object> arguments,
			params string[] requiredCapabilites);
		Task<int> Enqueue(PriorityQueueElement priorityElement);
		IDictionary<string, IPriorityQueueAction> PriorityQueueActions { get; }
		IDictionary<IComparable, QueueDispatcher> PriorityQueues { get; }

		int SerializeQueueGraph(IPriorityQueueArgumentStore priorityQueueArgumentStore,
			PriorityQueueElement priorityElement);

		Task<bool> DispatchHere(Guid queueId);
	}
}