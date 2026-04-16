//using JetBrains.Annotations;
//using MyWorksheet.Shared.WebApi;
//using MyWorksheet.Website.Server.Shared.Hubs.HubInfos.Worksheet;

//namespace MyWorksheet.Website.Server.Shared.Hubs.Hubs.Worksheet
//{
//	[HubName("WorksheetHub")]
//	[RevokableAuthorize]
//	[PublicAPI]
//	public class WorksheetHub : Hub
//	{
//		private WorksheetHubInfo _hubAccess;

//		public WorksheetHub()
//		{
//			_hubAccess = AccessElement<WorksheetHubInfo>.Instance;
//		}

//		public void RegisterTrackerChanged(Guid worksheetId)
//		{
//			_hubAccess.RegisterTrackerChanged(base.Context.ConnectionId, worksheetId);
//		}

//		public void UnRegisterTrackerChanged(Guid worksheetId)
//		{
//			_hubAccess.UnRegisterTrackerChanged(base.Context.ConnectionId, worksheetId);
//		}
//	}
//}