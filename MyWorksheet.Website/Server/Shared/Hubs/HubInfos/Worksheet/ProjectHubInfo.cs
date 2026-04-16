using System.Threading.Tasks;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using MyWorksheet.Website.Shared.ViewModels.Notifications;

namespace MyWorksheet.Website.Server.Shared.Hubs.HubInfos.Worksheet;
//public class ProjectHubInfo : HubAccess
//{
//	public override Type HubType { get; } = typeof(ProjectHub);

//	public void RegisterWorksheetItemChanged(string contextConnectionId, Guid projectId)
//	{
//		HubContext.Groups.Add(contextConnectionId, "WorksheetItem_Changed" + projectId);
//	}

//	public void UnRegisterWorksheetItemChanged(string contextConnectionId, Guid projectId)
//	{
//		HubContext.Groups.Remove(contextConnectionId, "WorksheetItem_Changed" + projectId);
//	}

//	public void SendWorksheetItemChanged(Guid projectId, WorksheetItemModel worksheetItem, ActionTypes actionType)
//	{
//		HubContext.Clients.Group("WorksheetItem_Changed" + projectId)
//			.WorksheetItemChanged(new StandardNotification<WorksheetItemModel>
//			{
//				Data = worksheetItem, 
//				Mode = actionType
//			});
//	}

//	public void RegisterProjectChanged(string contextConnectionId, Guid projectId)
//	{
//		HubContext.Groups.Add(contextConnectionId, "Project_Changed" + projectId);
//	}

//	public void UnRegisterProjectChanged(string contextConnectionId, Guid projectId)
//	{
//		HubContext.Groups.Remove(contextConnectionId, "Project_Changed" + projectId);
//	}

//	public void SendProjectChanged(Guid projectId, GetProjectModel worksheetItem, ActionTypes actionType)
//	{
//		HubContext.Clients.Group("Project_Changed" + projectId)
//			.Changed(new StandardNotification<GetProjectModel>
//			{
//				Data = worksheetItem, 
//				Mode = actionType
//			});
//	}
//}

public interface IProjectHub
{
    Task WorksheetItemChanged(StandardNotification<WorksheetItemModel> data);
    Task Changed(StandardNotification<GetProjectModel> data);
}