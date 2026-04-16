using System;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Services.ExecuteLater.ExecuteLater.ArgumentStore;

namespace MyWorksheet.Website.Server.Services.ExecuteLater.ExecuteLater.Contracts
{
	public interface IPriorityQueueArgumentStore
	{
		Task<PriorityQueueElement> GetFromStore(Guid queueItemId);
		Guid[] GetChilds(Guid queueItemId);
		Guid SetInStore(PriorityQueueElement element);
		Guid SetInStore(PriorityQueueElement element, Guid childOf);
		void SetFailed(Guid taskQueueItemId, Exception exception);
		void SetDone(Guid taskQueueItemId);
	}
}