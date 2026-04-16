using System.Threading.Tasks;
using MyWorksheet.Website.Server.Shared.Helper;
using MyWorksheet.Website.Shared.ViewModels.Notifications;

namespace MyWorksheet.Website.Server.Shared.Hubs.HubInfos.Worksheet;
//public class WorksheetHubInfo : HubAccess
//{
//	public override Type HubType { get; } = typeof(WorksheetHub);

//	public void RegisterTrackerChanged(string contextConnectionId, Guid worksheetId)
//	{
//		HubContext.Groups.Add(contextConnectionId, "Tracker_Changed" + worksheetId);
//	}

//	public void UnRegisterTrackerChanged(string contextConnectionId, Guid worksheetId)
//	{
//		HubContext.Groups.Remove(contextConnectionId, "Tracker_Changed" + worksheetId);
//	}

//	public void SendTrackerChanged(Guid worksheetId, EasyWorksheetTimeItem TrackerItem, ActionTypes actionType)
//	{
//		HubContext.Clients.Group("Tracker_Changed" + worksheetId)
//			.TrackerChanged(new StandardNotification<EasyWorksheetTimeItem>
//			{
//				Data = TrackerItem, 
//				Mode = actionType
//			});
//	}
//}

public interface IWorksheetHub
{
    Task TrackerChanged(StandardNotification<EasyWorksheetTimeItem> data);
}