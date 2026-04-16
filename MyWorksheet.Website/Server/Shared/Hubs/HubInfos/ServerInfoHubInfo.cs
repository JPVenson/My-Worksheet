//using System;
//using MyWorksheet.Webpage.Hubs;

//namespace MyWorksheet.Website.Server.Shared.Hubs.HubInfos
//{
//	public class ServerInfoHubInfo : HubAccess
//	{
//		public ServerInfoHubInfo()
//		{
//		}

//		public override Type HubType { get; } = typeof(ServerInfoHub);

//		public void RegisterServerInfoHubChanged(string contextConnectionId)
//		{
//			HubContext.Groups.Add(contextConnectionId, "Changed");
//		}

//		public void UnRegisterServerInfoHubChanged(string contextConnectionId)
//		{
//			HubContext.Groups.Remove(contextConnectionId, "Changed");
//		}

//		public void SendServerInfoHubChanged<T>(string key, T value)
//		{
//			HubContext.Clients.Group("Changed").ServerInfoChanged(new
//			{
//				DateSend = DateTime.UtcNow,
//				Key = key,
//				Value = value
//			});
//		}
//	}
//}