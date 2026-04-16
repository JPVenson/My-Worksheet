//using MyWorksheet.Hubs;
//using MyWorksheet.Shared.WebApi;
//using MyWorksheet.Website.Server.Shared.Hubs.HubInfos;
//using MyWorksheet.Website.Server.Util.Auth;

//namespace MyWorksheet.Webpage.Hubs
//{
//	[RevokableAuthorize]
//	public class WebhookLogHub : Hub
//	{
//		private readonly WebhookHubInfo _instance;

//		public WebhookLogHub()
//		{
//			_instance = AccessElement<WebhookHubInfo>.Instance;
//		}

//		[Authorize]
//		public void RegisterHistoryChanged()
//		{
//			_instance.RegisterHistoryChanged(base.Context.ConnectionId, base.Context.User.Identity.GetUserId());
//		}

//		[Authorize]
//		public void UnRegisterHistoryChanged()
//		{
//			_instance.UnRegisterHistoryChanged(base.Context.ConnectionId, base.Context.User.Identity.GetUserId());
//		}
//	}
//}