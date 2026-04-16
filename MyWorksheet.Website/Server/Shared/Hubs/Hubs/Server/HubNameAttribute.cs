using System;

namespace MyWorksheet.Website.Server.Shared.Hubs.Hubs.Server;
//[HubName("ServerCommunication")]
//[RevokableAuthorize]
//public class ServerCommunicationHub : Hub<IServerCommunicationHub>
//{
//	public void RegisterServerChanged()
//	{
//		Groups.AddToGroupAsync(Context.ConnectionId, "Changed");
//	}

//	public void UnRegisterServerChanged()
//	{
//		Groups.RemoveFromGroupAsync(Context.ConnectionId, "Changed");
//	}
//}

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
sealed class HubNameAttribute : Attribute
{
    public string Name { get; }

    public HubNameAttribute(string name)
    {
        Name = name;
    }
}