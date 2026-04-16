//using JetBrains.Annotations;
//using MyWorksheet.Shared.WebApi;
//using MyWorksheet.Website.Server.Shared.Hubs.HubInfos.Worksheet;

//namespace MyWorksheet.Website.Server.Shared.Hubs.Hubs.Worksheet
//{
//	[HubName("WorksheetItemHub")]
//	[RevokableAuthorize]
//	[PublicAPI]
//	public class WorksheetItemHub : Hub
//	{
//		private WorksheetItemHubInfo _hubAccess;

//		public WorksheetItemHub()
//		{
//			_hubAccess = AccessElement<WorksheetItemHubInfo>.Instance;
//		}

//		public void RegisterWorksheetItemChanged(Guid worksheetId)
//		{
//			_hubAccess.RegisterWorksheetItemChanged(base.Context.ConnectionId, worksheetId);
//		}

//		public void UnRegisterWorksheetItemChanged(Guid worksheetId)
//		{
//			_hubAccess.UnRegisterWorksheetItemChanged(base.Context.ConnectionId, worksheetId);
//		}
//	}
//}