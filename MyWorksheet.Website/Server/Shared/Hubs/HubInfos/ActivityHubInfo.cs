using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Activity;

namespace MyWorksheet.Website.Server.Shared.Hubs.HubInfos;
//public class ActivityHubInfo : HubAccess
//{
//	public ActivityHubInfo()
//	{
//	}

//	public override Type HubType { get; } = typeof(ActivityHub);

//	public void RegisterActivityChanged(string contextConnectionId, string getUserId)
//	{
//		HubContext.Groups.Add(contextConnectionId, "Changed_" + getUserId);
//	}

//	public void UnRegisterActivityChanged(string contextConnectionId, string getUserId)
//	{
//		HubContext.Groups.Remove(contextConnectionId, "Changed_" + getUserId);
//	}

//	public void SendActivityChanged(long activityIdAppUser, UserActivity activity, ExternalActivityAction created)
//	{
//		HubContext.Clients.Group("Changed_" + activityIdAppUser.ToString()).ActivityChanged(new { Activity = activity, Mode = created.ToString() });
//	}
//}

public interface IWorksheetItemHub
{
    Task ActivityChanged(UserActivity userActivity, ExternalActivityAction mode);
}