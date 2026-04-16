//using MyWorksheet.Hubs;
//using MyWorksheet.Shared.WebApi;
//using MyWorksheet.Website.Server.Shared.Hubs.HubInfos;
//using MyWorksheet.Website.Server.Util.Auth;

//namespace MyWorksheet.Webpage.Hubs
//{
//	[RevokableAuthorize]
//	public class ReportHub : Hub
//	{
//		private readonly ReportHubInfo _hubInfo;

//		public ReportHub()
//		{
//			_hubInfo = AccessElement<ReportHubInfo>.Instance;
//		}

//		public void RegisterReportGenerationChanged(Guid reportId)
//		{
//			_hubInfo.RegisterReportGenerationChanged(Context.ConnectionId, Context.User.GetUserId(), reportId);
//		}

//		public void UnRegisterReportGenerationChanged(Guid reportId)
//		{
//			_hubInfo.UnRegisterReportGenerationChanged(Context.ConnectionId, Context.User.GetUserId(), reportId);
//		}

//		public void RegisterReportChanged(Guid reportId)
//		{
//			_hubInfo.RegisterReportChanged(Context.ConnectionId, Context.User.GetUserId(), reportId);
//		}

//		public void UnRegisterReportChanged(Guid reportId)
//		{
//			_hubInfo.UnRegisterReportChanged(Context.ConnectionId, Context.User.GetUserId(), reportId);
//		}
//	}
//}