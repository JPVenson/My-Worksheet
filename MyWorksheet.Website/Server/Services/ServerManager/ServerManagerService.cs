using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using MyWorksheet.Helper.Db;
using MyWorksheet.Shared.Helper;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.ObjectChanged;
using MyWorksheet.Website.Server.Settings;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using MyWorksheet.Website.Shared.Services.Activation;
using MyWorksheet.Website.Shared.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ServiceLocator.Attributes;
using MyWorksheet.Website.Shared.Services;

namespace MyWorksheet.Website.Server.Services.ServerManager;

public interface IServerManagerService
{
    KnownServer Self { get; }
    void GoOnline(IHostApplicationLifetime applicationLifetime);
    void RefreshKnownServers(bool bubbleOnline);
    KnownServer[] GetKnownServers();
    IEnumerable<KnownServer> GetServerWith(params string[] capabilities);
}

[SingletonService(typeof(IServerManagerService))]
public class ServerManagerService : RequireInit, IServerManagerService
{
    private readonly IDbContextFactory<MyworksheetContext> _dbFactory;
    private readonly IOptions<MyWorksheetSettings> _myWorksheetSettings;
    private readonly IOptions<AppServerSettings> _serverSettings;
    private readonly ILogger<ServerManagerService> _logger;
    private readonly ActivatorService _activatorService;
    private readonly ObjectChangedService _objectChangedService;

    public ServerManagerService(IDbContextFactory<MyworksheetContext> dbFactory,
        IOptions<MyWorksheetSettings> myWorksheetSettings,
        IOptions<AppServerSettings> serverSettings,
        ILogger<ServerManagerService> logger,
        ActivatorService activatorService,
        ObjectChangedService objectChangedService)
    {
        _dbFactory = dbFactory;
        _myWorksheetSettings = myWorksheetSettings;
        _serverSettings = serverSettings;
        _logger = logger;
        _activatorService = activatorService;
        _objectChangedService = objectChangedService;
        KnownServers = [];
    }

    public List<KnownServer> KnownServers { get; }

    public Realm Realm { get; private set; }
    public KnownServer Self { get; private set; }

    public IEnumerable<KnownServer> GetServerWith(params string[] capabilities)
    {
        return KnownServers.Where(e => capabilities.All(f => e.ServerCapabilities.Any(w => w.Name.Equals(f))));
    }

    public override ValueTask InitAsync(IServiceProvider services)
    {
        GoOnline(services.GetService<IHostApplicationLifetime>());

        return base.InitAsync(services);
    }

    public void GoOnline(IHostApplicationLifetime applicationLifetime)
    {
        using var db = _dbFactory.CreateDbContext();
        var name = _myWorksheetSettings.Value.Server.Instance.Name;
        var type = _myWorksheetSettings.Value.Server.Instance.Type;
        var realm = _myWorksheetSettings.Value.Server.Instance.Realm;

        var primaryRealm = _serverSettings.Value.Realm.PrimaryRealm;

        Realm = db.Realms
            .Where(f => f.Named == realm)
            .FirstOrDefault();
        if (Realm == null)
        {
            Realm = new Realm
            {
                Named = realm
            };
            db.Add(Realm);
        }

        var isServerKnown = db.Processors
            .Where(f => f.IdRealm == Realm.RealmId)
            .Where(f => f.ExternalIdentity == name)
            .FirstOrDefault();

        if (isServerKnown == null)
        {
            isServerKnown = new Processor
            {
                IdRealm = Realm.RealmId,
                ExternalIdentity = name,
                IpOrHostname = primaryRealm,
                Role = type,
                Online = true,
                AuthKey = Guid.NewGuid().ToString("N")
            };
            db.Add(isServerKnown);
        }
        else
        {
            _logger.LogWarning("Ether the server has not successfully shutdown or there " +
                               $"is a missconfiguration for server '{isServerKnown.Role}:{isServerKnown.ExternalIdentity}'");

            db.Processors.Where(e => e.ProcessorId == isServerKnown.ProcessorId)
                .ExecuteUpdate(f =>
                    f.SetProperty(e => e.IpOrHostname, primaryRealm)
                    .SetProperty(e => e.IdRealm, Realm.RealmId)
                    .SetProperty(e => e.Role, type)
                    .SetProperty(e => e.AuthKey, Guid.NewGuid().ToString("N"))
                    .SetProperty(e => e.Online, true)
                );
        }

        var capabilities = new List<ProcessorCapability>();

        var capabilitiesType = _activatorService.ActivateType<EnumerationHelper>();
        foreach (var reportCapability in capabilitiesType.Values)
        {
            capabilities.AddRange(reportCapability.ReportCapabilities());
        }

        capabilities.ForEach(e => e.IdProcessor = isServerKnown.ProcessorId);

        var existing = db.ProcessorCapabilities
            .Where(f => f.IdProcessor == isServerKnown.ProcessorId)
            .ToArray();

        db.DoCreateDeleteOrUpdate(existing, capabilities, e => e.Name);

        db.SaveChanges();

        Self = new KnownServer(isServerKnown,
            db.ProcessorCapabilities.Where(f => f.IdProcessor == isServerKnown.ProcessorId).ToArray());

        RefreshKnownServers(true);

        applicationLifetime.ApplicationStopping.Register(() => { GoOffline(); });
    }

    private class EnumerationHelper
    {
        public IEnumerable<IReportCapability> Values { get; }

        public EnumerationHelper(IEnumerable<IReportCapability> values)
        {
            Values = values;
        }
    }

    public void RefreshKnownServers(bool bubbleOnline)
    {
        using var db = _dbFactory.CreateDbContext();
        var knownServers = db.Processors
            .Where(f => f.IdRealm == Realm.RealmId)
            .ToArray();

        KnownServers.Clear();
        foreach (var processor in knownServers)
        {
            var server = new KnownServer(processor,
                db.ProcessorCapabilities.Where(f => f.IdProcessor == processor.ProcessorId).ToArray());
            server.Online = processor.Online;
            KnownServers.Add(server);
            if (processor.ExternalIdentity != Self.Name && bubbleOnline)
            {
                server.SendMeOnline().AttachNonVerboseAsyncHandler();
                server.AttachStatusListener().AttachNonVerboseAsyncHandler();
            }
        }
    }

    public KnownServer[] GetKnownServers()
    {
        return KnownServers.ToArray();
    }

    public void GoOffline()
    {
        using var db = _dbFactory.CreateDbContext();
        var name = _myWorksheetSettings.Value.Server.Instance.Name;
        db.Processors.Where(e => e.ExternalIdentity == name).ExecuteUpdate(f => f.SetProperty(e => e.Online, false));

        _objectChangedService.SendObjectChanged(ChangeEventTypes.Removed, Self, null).Wait();
        //AccessElement<ServerCommunicationHubInfo>.Instance.SendRegisterServerChanged(Self.Name);
    }
}

