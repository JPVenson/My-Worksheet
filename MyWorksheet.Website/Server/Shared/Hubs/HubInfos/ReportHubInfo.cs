//using System;
//using MyWorksheet.Webpage.Hubs;
//using MyWorksheet.Website.Server.Services.Reporting;

//namespace MyWorksheet.Website.Server.Shared.Hubs.HubInfos
//{

//	public class ReportHubInfo : HubAccess
//	{
//		public ReportHubInfo()
//		{
//			_templateManager = IoC.Resolve<ITextTemplateManager>();
//			if (_templateManager != null)
//			{
//				_templateManager.ReportPositionChanged += _templateManager_ReportPositionChanged;
//			}
//		}

//		private void _templateManager_ReportPositionChanged(Guid reportId, int newPosition, int executingUser)
//		{
//			TriggerReportGenerationChanged(executingUser, reportId, newPosition);
//		}

//		private readonly ITextTemplateManager _templateManager;

//		public override Type HubType { get; } = typeof(ReportHub);

//		public void RegisterReportGenerationChanged(string connectionId, Guid userId, Guid reportId)
//		{
//			HubContext.Groups.Add(connectionId, userId + "_" + reportId);
//		}

//		public void UnRegisterReportGenerationChanged(string connectionId, Guid userId, Guid reportId)
//		{
//			HubContext.Groups.Remove(connectionId, userId + "_" + reportId);
//		}	

//		public void RegisterReportChanged(string connectionId, Guid userId, Guid reportId)
//		{
//			HubContext.Groups.Add(connectionId, "R_" + reportId);
//		}

//		public void UnRegisterReportChanged(string connectionId, Guid userId, Guid reportId)
//		{
//			HubContext.Groups.Remove(connectionId, "R_" + reportId);
//		}

//		public void TriggerReportGenerationChanged(Guid userId, Guid reportId, int position)
//		{
//			HubContext.Clients.Group(userId + "_" + reportId).PositionChanged(new
//			{
//				position = position,
//				reportId
//			});
//		}

//		public void TriggerReportChanged(Guid reportId)
//		{			
//			HubContext.Clients.Group("R_" + reportId).Changed(reportId);
//		}
//	}
//}