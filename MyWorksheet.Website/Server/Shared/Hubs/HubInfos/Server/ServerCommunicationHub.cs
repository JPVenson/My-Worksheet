namespace MyWorksheet.Website.Server.Shared.Hubs.HubInfos.Server;
//public class ServerCommunicationHubInfo : HubAccess
//{
//	public override Type HubType { get; } = typeof(ServerCommunicationHub);

//	public void RegisterServerChanged(string contextConnectionId)
//	{
//		HubContext.Groups.Add(contextConnectionId, "Changed");
//	}

//	public void UnRegisterServerChanged(string contextConnectionId)
//	{
//		HubContext.Groups.Remove(contextConnectionId, "Changed");
//	}

//	public void SendRegisterServerChanged(string name)
//	{
//		HubContext.Clients.Group("Changed").ServerChanged(name);
//	}
//}

public interface IServerCommunicationHub
{
    void ServerChanged(string name);
}