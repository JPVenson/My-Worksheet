//using System;
//using MyWorksheet.Public.Models.ViewModel.RemoteStorage;
//using MyWorksheet.Webpage.Hubs;

//namespace MyWorksheet.Website.Server.Shared.Hubs.HubInfos
//{
//	public class RemoteStorageHubInfo : HubAccess
//	{
//		public RemoteStorageHubInfo()
//		{
//			RemoteStorageService = IoC.Resolve<RemoteStorageService>();
//		}

//		public override Type HubType { get; } = typeof(RemoteStorageHub);
//		public RemoteStorageService RemoteStorageService { get; private set; }

//		public void RegisterRemoteStorage(string contextConnectionId, string accessKey)
//		{
//			HubContext.Groups.Add(contextConnectionId, "Command_" + accessKey);
//			RemoteStorageService.StorageStatusChange(true, accessKey);
//		}

//		public void UnRegisterRemoteStorage(string contextConnectionId, string accessKey)
//		{
//			HubContext.Groups.Remove(contextConnectionId, "Command_" + accessKey);
//			RemoteStorageService.StorageStatusChange(false, accessKey);
//		}

//		public void SendCommand(Commands command, string accessKey, string operationKey)
//		{
//			HubContext.Clients.Group("Command_" + accessKey).Command(new
//			{
//				Command = command,
//				Operation = operationKey
//			});
//		}
//	}
//}