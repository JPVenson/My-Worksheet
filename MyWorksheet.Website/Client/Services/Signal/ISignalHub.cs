using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Auth;
using Microsoft.AspNetCore.SignalR.Client;

namespace MyWorksheet.Website.Client.Services.Signal;

public interface ISignalHub
{
    string HubName { get; }
    bool CanConnect(CurrentUserStore currentUserStore);
    void Register(HubConnection connection);
    Task Init(HubConnection connection);
    Task OnReconnect(HubConnection connection);
}