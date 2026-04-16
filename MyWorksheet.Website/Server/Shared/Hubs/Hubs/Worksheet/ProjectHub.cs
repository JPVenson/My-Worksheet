//using System.Threading.Tasks;
//using JetBrains.Annotations;
//using MyWorksheet.Public.Models.ViewModel.ApiResultModels.Worksheet;
//using MyWorksheet.Public.Models.ViewModel.Notifications;
//using MyWorksheet.Shared.WebApi;
//using MyWorksheet.Website.Server.Shared.Hubs.HubInfos.Worksheet;
//using MyWorksheet.Website.Server.Shared.Hubs.Hubs.Server;
//using Microsoft.AspNetCore.SignalR;

//namespace MyWorksheet.Website.Server.Shared.Hubs.Hubs.Worksheet
//{
//	[HubName("ProjectHub")]
//	[RevokableAuthorize]
//	[PublicAPI]
//	public class ProjectHub : Hub<IProjectHub>
//	{
//		public async Task RegisterWorksheetItemChanged(Guid projectId)
//		{
//			await Groups.AddToGroupAsync(Context.ConnectionId, "WorksheetItem_Changed" + projectId);
//		}

//		public async Task UnRegisterWorksheetItemChanged(Guid projectId)
//		{
//			await Groups.RemoveFromGroupAsync(Context.ConnectionId, "WorksheetItem_Changed" + projectId);
//		}

//		public async Task RegisterProjectChanged(Guid projectId)
//		{
//			await Groups.AddToGroupAsync(Context.ConnectionId, "Project_Changed" + projectId);
//		}

//		public async Task UnRegisterProjectChanged(Guid projectId)
//		{
//			await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Project_Changed" + projectId);
//		}

//		public void SendWorksheetItemChanged(Guid projectId, WorksheetItemModel worksheetItem, ActionTypes actionType)
//		{
//			Clients.Group("WorksheetItem_Changed" + projectId)
//				.WorksheetItemChanged(new StandardNotification<WorksheetItemModel>
//				{
//					Data = worksheetItem,
//					Mode = actionType
//				});
//		}

//		public void SendProjectChanged(Guid projectId, GetProjectModel worksheetItem, ActionTypes actionType)
//		{
//			Clients.Group("Project_Changed" + projectId)
//				.Changed(new StandardNotification<GetProjectModel>
//				{
//					Data = worksheetItem,
//					Mode = actionType
//				});
//		}
//	}
//}
//}