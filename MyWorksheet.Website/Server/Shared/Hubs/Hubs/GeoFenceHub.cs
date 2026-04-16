//using MyWorksheet.Hubs;
//using MyWorksheet.Shared.WebApi;
//using MyWorksheet.Website.Server.Util.Auth;

//namespace MyWorksheet.Webpage.Hubs
//{
//	[RevokableAuthorize]
//	public class GeoFenceHub : Hub
//	{
//		private readonly GeoFenceHubInfo _hubInfo;

//		public GeoFenceHub()
//		{
//			_hubInfo = AccessElement<GeoFenceHubInfo>.Instance;
//		}

//		public void RegisterGeoFenceChanged(int? geoFenceId = null)
//		{
//			_hubInfo.RegisterGeoFenceChanged(Context.ConnectionId, Context.User.GetUserId(), geoFenceId);
//		}

//		public void UnRegisterGeoFenceChanged(int? geoFenceId = null)
//		{
//			_hubInfo.UnRegisterGeoFenceChanged(Context.ConnectionId, Context.User.GetUserId(), geoFenceId);
//		}
//	}
//}