//using System;
//using MyWorksheet.Webpage.Hubs;

//namespace MyWorksheet.Website.Server.Shared.Hubs.HubInfos
//{
//	public class StorageEntryChangedHubInfo : HubAccess
//	{
//		public override Type HubType { get; } = typeof(StorageEntryChangedHub);

//		public void RegisterChanged(string contextConnectionId, Guid userId)
//		{
//			HubContext.Groups.Add(contextConnectionId, "Changed" + userId);
//		}

//		public void UnRegisterChanged(string contextConnectionId, Guid userId)
//		{
//			HubContext.Groups.Remove(contextConnectionId, "Changed" + userId);
//		}

//		public void SendChanged(Guid storageId, Guid userId)
//		{
//			HubContext.Clients.Group("Changed" + userId).EntryChanged(new
//			{
//				StorageId = storageId
//			});
//		}
//	}
//}