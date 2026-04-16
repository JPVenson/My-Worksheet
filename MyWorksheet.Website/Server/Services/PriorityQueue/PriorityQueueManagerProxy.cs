using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater;
using MyWorksheet.Website.Server.Services.ServerManager;

namespace MyWorksheet.Shared.Services.PriorityQueue;

public interface IServerPriorityQueueManager : IPriorityQueueManager, IReportCapability
{

}

//[SingletonService(typeof(IServerPriorityQueueManager), typeof(IReportCapability))]
//public class PriorityQueueManagerProxy : RequireInit, IServerPriorityQueueManager
//{
//	private readonly IPriorityQueueArgumentStore _priorityQueueArgumentStore;
//	private readonly ActivatorService _activatorService;

//	public PriorityQueueManagerProxy(
//		PriorityQueueOptions options,
//		IAppMetricsLogger appMetricsLogger, 
//		IAppLogger appLogger, 
//		IPriorityQueueArgumentStore priorityQueueArgumentStore,
//		IServerManagerService serverManagerService,
//		ActivatorService activatorService)
//	{
//		_priorityQueueArgumentStore = priorityQueueArgumentStore;
//		_activatorService = activatorService;
//		PriorityQueueManager = new PriorityQueueManager(options, appMetricsLogger, appLogger,priorityQueueArgumentStore, _activatorService);
//		ServerManagerService = serverManagerService;
//	}

//	public override ValueTask InitAsync(IServiceProvider services)
//	{
//		return (PriorityQueueManager as PriorityQueueManager).InitAsync(services);
//	}

//	public IServerManagerService ServerManagerService { get; private set; }

//	public IPriorityQueueManager PriorityQueueManager { get; private set; }
//	public void LoadFromAttribute(Assembly assemblyToLoadFrom)
//	{
//		PriorityQueueManager.LoadFromAttribute(assemblyToLoadFrom);
//	}

//	public Task<Guid> Enqueue(IComparable priotity, string actionKey, Guid userId,
//		IDictionary<string, object> arguments, params string[] requiredCapabilites)
//	{
//		return Enqueue(new PriorityQueueElement(priotity, userId, actionKey, arguments, new PriorityQueueElement[0])
//		{
//			CapabilitiesRequired = requiredCapabilites
//		});
//	}

//	public async Task<int> Enqueue(PriorityQueueElement priorityElement)
//	{
//		if (priorityElement.CapabilitiesRequired != null 
//		    && priorityElement.CapabilitiesRequired.Any(e => !ServerManagerService.Self.ServerCapabilities.Any(f => f.Name.Equals(e))))
//		{
//			var storeId = this.SerializeQueueGraph(_priorityQueueArgumentStore, priorityElement);

//			foreach (var knownServer 
//				in ServerManagerService.GetServerWith(priorityElement.CapabilitiesRequired.Concat(new string[]
//					{
//						priorityElement.ActionKey
//					}).ToArray())
//				.Where(f => f.Online))
//			{
//				if (await knownServer.SendProgressQueueItem(storeId))
//				{
//					break;
//				}
//			}

//			return storeId;
//		}

//		return await PriorityQueueManager.Enqueue(priorityElement);
//	}

//	public IDictionary<string, IPriorityQueueAction> PriorityQueueActions
//	{
//		get { return PriorityQueueManager.PriorityQueueActions; }
//	}

//	public IDictionary<IComparable, QueueDispatcher> PriorityQueues
//	{
//		get { return PriorityQueueManager.PriorityQueues; }
//	}

//	public int SerializeQueueGraph(IPriorityQueueArgumentStore priorityQueueArgumentStore, PriorityQueueElement priorityElement)
//	{
//		return PriorityQueueManager.SerializeQueueGraph(priorityQueueArgumentStore, priorityElement);
//	}

//	public async Task<bool> DispatchHere(Guid queueId)
//	{
//		return await PriorityQueueManager.DispatchHere(queueId);
//	}

//	public ProcessorCapability[] ReportCapabilities()
//	{
//		return PriorityQueueManager.PriorityQueueActions.Keys
//			.Select(e => new ProcessorCapability()
//			{
//				Name = e,
//				Value = "",
//				IsEnabled = true
//			})
//			.ToArray();
//	}
//}