//using MyWorksheet.Hubs;
//using MyWorksheet.Shared.WebApi;
//using MyWorksheet.Website.Server.Shared.Hubs.HubInfos;

//namespace MyWorksheet.Webpage.Hubs
//{
//	//[Authorize(Roles = Roles.AdminRoleName)]
//	[RevokableAuthorize]
//	public class ServerInfoHub : Hub
//	{
//		private readonly ServerInfoHubInfo _instance;

//		public ServerInfoHub()
//		{
//			_instance = AccessElement<ServerInfoHubInfo>.Instance;
//		}

//		public void RegisterServerInfoHubChanged()
//		{
//			_instance.RegisterServerInfoHubChanged(base.Context.ConnectionId);
//		}

//		public void UnRegisterServerInfoHubChanged()
//		{
//			_instance.UnRegisterServerInfoHubChanged(base.Context.ConnectionId);
//		}
//	}
//}