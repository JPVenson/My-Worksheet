using System.Threading.Tasks;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using MyWorksheet.Website.Shared.ViewModels.Notifications;

namespace MyWorksheet.Website.Server.Shared.Hubs.HubInfos.Worksheet;
//public class WorksheetItemHubInfo : HubAccess
//{
//	public override Type HubType { get; } = typeof(WorksheetItemHub);

//	public void RegisterWorksheetItemChanged(string contextConnectionId, Guid worksheetId)
//	{
//		HubContext.Groups.Add(contextConnectionId, "WorksheetItem_Changed" + worksheetId);
//	}

//	public void UnRegisterWorksheetItemChanged(string contextConnectionId, Guid worksheetId)
//	{
//		HubContext.Groups.Remove(contextConnectionId, "WorksheetItem_Changed" + worksheetId);
//	}

//	public void SendWorksheetItemChanged(Guid worksheetId, WorksheetItemModel worksheetItem, ActionTypes actionType)
//	{
//		HubContext.Clients.Group("WorksheetItem_Changed" + worksheetId)
//			.Changed(new StandardNotification<WorksheetItemModel>
//			{
//				Data = worksheetItem, 
//				Mode = actionType
//			});
//	}
//}

public interface IWorksheetItemHub
{
    Task TrackerChanged(StandardNotification<WorksheetItemModel> data);
}