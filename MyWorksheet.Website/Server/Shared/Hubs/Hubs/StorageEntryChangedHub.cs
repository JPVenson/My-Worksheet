//using MyWorksheet.Hubs;
//using MyWorksheet.Shared.WebApi;
//using MyWorksheet.Website.Server.Shared.Hubs.HubInfos;
//using MyWorksheet.Website.Server.Util.Auth;

//namespace MyWorksheet.Webpage.Hubs
//{
//	[HubName("StorageEntryChangedHub")]
//	[RevokableAuthorize()]
//	public class StorageEntryChangedHub : Hub
//	{
//		private readonly StorageEntryChangedHubInfo _instance;


//		[InjectionConstructor]
//		public StorageEntryChangedHub()
//		{
//			_instance = AccessElement<StorageEntryChangedHubInfo>.Instance;
//		}

//		public void RegisterChanged()
//		{
//			_instance.RegisterChanged(base.Context.ConnectionId, Context.User.GetUserId());
//		}

//		public void UnRegisterChanged()
//		{
//			_instance.UnRegisterChanged(base.Context.ConnectionId, Context.User.GetUserId());
//		}
//	}
//}