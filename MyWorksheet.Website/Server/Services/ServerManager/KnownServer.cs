using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;

namespace MyWorksheet.Website.Server.Services.ServerManager;

public class KnownServer
{
    private readonly string _authKey;

    private HttpClient _httpClient;
    //private HubConnection _signalRConnection;

    public KnownServer(Processor processorEntity, ProcessorCapability[] capabilities)
    {
        _authKey = processorEntity.AuthKey;
        Name = processorEntity.ExternalIdentity;
        HostName = processorEntity.IpOrHostname;
        Type = processorEntity.Role;
        InstanceId = processorEntity.ProcessorId;
        ServerCapabilities = capabilities.Select(e => new ServerCapability
        {
            Value = e.Value,
            Name = e.Name
        });
        InitHttpClient();
        InitSignalR();
    }

    public string Name { get; }
    public string HostName { get; }
    public string Type { get; }
    public bool Online { get; set; }
    public Guid InstanceId { get; }

    public IEnumerable<ServerCapability> ServerCapabilities { get; }

    private void InitHttpClient()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(HostName, UriKind.RelativeOrAbsolute);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("x-sts-authKey", _authKey);
    }

    private void InitSignalR()
    {
        //_signalRConnection = new HubConnection(HostName, new Dictionary<string, string>
        //{
        //	{"token", _authKey},
        //	{"sts", "true"}
        //});
    }

    public async Task CheckOnline()
    {
        var httpResponseMessage = await _httpClient.PostAsync("/api/HealthCheckApi/Check", null);
        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            //AccessElement<ServerCommunicationHubInfo>.Instance.SendRegisterServerChanged(Name);
            Online = false;
            return;
        }

        if (await httpResponseMessage.Content.ReadAsStringAsync() != "ohh Heeeelloo there!")
        {
            //AccessElement<ServerCommunicationHubInfo>.Instance.SendRegisterServerChanged(Name);
            Online = false;
            return;
        }

        if (!Online)
        {
            //AccessElement<ServerCommunicationHubInfo>.Instance.SendRegisterServerChanged(Name);
        }

        Online = true;
    }

    public async Task SendMeOnline()
    {
        await _httpClient.PostAsync("/api/ServerApi/RegisterServer", null);
    }

    public async Task<bool> SendProgressQueueItem(Guid storeId)
    {
        return (await _httpClient.PostAsync(
            BuildUrl("/api/ServerApi/DispatchWorkOrder", new Tuple<string, string>("queueId", storeId.ToString())),
            null)).IsSuccessStatusCode;
    }

    private Uri BuildUrl(string url, params Tuple<string, string>[] arguments)
    {
        if (arguments.Any())
        {
            url = url + "?";

            foreach (var keyValuePair in arguments)
            {
                url += WebUtility.UrlEncode(keyValuePair.Item1) + "=" + WebUtility.UrlEncode(keyValuePair.Item2);
            }
        }

        return new Uri(url, UriKind.RelativeOrAbsolute);
    }

    private async Task StartSignalR()
    {
        //await _signalRConnection.Start();
    }

    public async Task AttachStatusListener()
    {
        await StartSignalR();
        //var hubProxy = _signalRConnection.CreateHubProxy(nameof(ServerCommunicationHub));
        //hubProxy.On<string>("ServerChanged", serverChanged => { CheckOnline().AttachNonVerboseAsyncHandler(); });
    }
}
