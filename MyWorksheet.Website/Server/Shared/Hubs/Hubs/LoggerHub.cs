using System.Threading.Tasks;
using MyWorksheet.Webpage.Helper.Roles;
using MyWorksheet.Website.Server.Shared.Hubs.Hubs.Server;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MyWorksheet.Website.Server.Shared.Hubs.Hubs;

[HubName("LoggerHub")]
[Authorize(Roles = Roles.AdminRoleName)]
public class LoggerHub : Hub<ILoggerHub>
{
    public async Task RegisterLog()
    {
        await base.Groups.AddToGroupAsync(base.Context.ConnectionId, "ALL");
    }

    public async Task UnRegisterLog()
    {
        await base.Groups.RemoveFromGroupAsync(base.Context.ConnectionId, "ALL");
    }
}