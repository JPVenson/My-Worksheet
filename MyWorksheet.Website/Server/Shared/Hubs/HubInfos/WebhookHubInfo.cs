//using System;
//using MyWorksheet.Webpage.Hubs;

//namespace MyWorksheet.Website.Server.Shared.Hubs.HubInfos
//{
//	public class WebhookHubInfo : HubAccess
//	{
//		public WebhookHubInfo()
//		{

//		}

//		public void RegisterHistoryChanged(string connectionId, string userId)
//		{
//			HubContext.Groups.Add(connectionId, userId);
//		}

//		public void UnRegisterHistoryChanged(string connectionId, string userId)
//		{
//			HubContext.Groups.Remove(connectionId, userId);
//		}

//		public void SendHistoryChanged(Guid userId, Guid hookId)
//		{
//			HubContext.Clients.Group(userId.ToString()).HistoryChanged(new { hookId = hookId });
//		}

//		public override Type HubType { get; } = typeof(WebhookLogHub);
//	}
//}