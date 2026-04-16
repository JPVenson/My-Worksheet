//using MyWorksheet.Shared.WebApi;
//using MyWorksheet.Website.Server.Util.Auth;
//using Microsoft.AspNet.SignalR.Hubs;
//using Microsoft.AspNetCore.SignalR;

//namespace MyWorksheet.Hubs.Hubs
//{
//	[HubName("ActivityHub")]
//	[RevokableAuthorize]
//	public class ActivityHub : Hub
//	{
//		private readonly ActivityHubInfo _instance;

//		public ActivityHub()
//		{
//			_instance = AccessElement<ActivityHubInfo>.Instance;
//		}

//		public void RegisterActivityChanged()
//		{
//			_instance.RegisterActivityChanged(base.Context.ConnectionId, base.Context.User.Identity.GetUserId());
//		}

//		public void UnRegisterActivityChanged()
//		{
//			_instance.UnRegisterActivityChanged(base.Context.ConnectionId, base.Context.User.Identity.GetUserId());
//		}
//	}
//}