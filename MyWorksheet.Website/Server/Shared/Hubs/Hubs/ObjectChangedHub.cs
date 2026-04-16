using System.Threading.Tasks;
using MyWorksheet.Website.Server.Shared.Hubs.Hubs.Server;
using MyWorksheet.Website.Server.Util.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MyWorksheet.Website.Server.Shared.Hubs.Hubs;

[HubName("ObjectChangedHub")]
[Authorize()]
public class ObjectChangedHub : Hub<IObjectChangedHub>
{
    public async Task<string> RegisterChangeTracking()
    {
        await base.Groups.AddToGroupAsync(base.Context.ConnectionId, "ChangeTracking_" + Context.User.GetUserId());
        await base.Groups.AddToGroupAsync(base.Context.ConnectionId, "ALL");
        return Context.ConnectionId;
    }

    public async Task UnRegisterChangeTracking()
    {
        await base.Groups.RemoveFromGroupAsync(base.Context.ConnectionId, "ChangeTracking_" + Context.User.GetUserId());
        await base.Groups.RemoveFromGroupAsync(base.Context.ConnectionId, "ALL");
    }
}