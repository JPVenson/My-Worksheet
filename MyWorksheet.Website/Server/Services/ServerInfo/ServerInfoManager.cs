using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Services.CdnFallback;
using MyWorksheet.Website.Server.Services.FileSystem;
using MyWorksheet.Website.Server.Services.ServerInfo.InfoItems;
using MyWorksheet.Website.Server.Services.ServerInfo.InfoItems.Drive;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using MyWorksheet.Website.Shared.Services.Activation;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Services.ServerInfo;

[SingletonService()]
public class ServerInfoManager
{
    private readonly ActivatorService _activatorService;
    private readonly ICdnFallbackService _cdnFallbackService;
    private readonly ILocalFileProvider _localFileProvider;
    private readonly ILogger<ServerInfoManager> _appLogger;

    public ServerInfoManager(ActivatorService activatorService,
        ICdnFallbackService cdnFallbackService,
        ILocalFileProvider localFileProvider,
        ILogger<ServerInfoManager> appLogger)
    {
        _activatorService = activatorService;
        _cdnFallbackService = cdnFallbackService;
        _localFileProvider = localFileProvider;
        _appLogger = appLogger;
        Timeout = TimeSpan.FromSeconds(60);
        if (Debugger.IsAttached)
        {
            Timeout = TimeSpan.FromHours(1);
        }
        ServerInfoListeners = [];
        ExternalListeners = [];
    }

    public ConcurrentBag<ServerInfoListener> ServerInfoListeners { get; set; }
    public List<Func<Action<string, object>, ServerInfoListener>> ExternalListeners { get; set; }

    public IEnumerable<string> RegisterServerListeners(Action<string, object> publishValue)
    {
        var errors = new List<string>();
        if (ServerInfoListeners.Any())
        {
            KeepAlive = true;
            return errors;
        }

        foreach (var externalListener in ExternalListeners)
        {
            try
            {
                ServerInfoListeners.Add(externalListener(publishValue));
            }
            catch (Exception e)
            {
                errors.Add(e.Message);
            }
        }

        //WINDOWS SPECIFIC
        //try
        //{
        //	ServerInfoListeners.Add(new ServerPullPerformanceCounterListener(publishValue, "total_cpu", new PerformanceCounter("Processor", "% Processor Time", "_Total")));
        //	ServerInfoListeners.Add(new ServerPullPerformanceCounterListener(publishValue, "total_memory", new PerformanceCounter("Memory", "Available MBytes")));
        //}
        //catch (Exception e)
        //{
        //	errors.Add(e.Message);
        //}
        try
        {
            ServerInfoListeners.Add(new ServerPullInfoListenerDelegate(publishValue,
                "process_memory",
                () => Process.GetCurrentProcess().WorkingSet64,
                _appLogger));

            TimeSpan lastCpuTime = Process.GetCurrentProcess().TotalProcessorTime;
            var lastCheck = DateTime.UtcNow;
            ServerInfoListeners.Add(new ServerPullInfoListenerDelegate(publishValue, "process_cpu",
                () =>
                {
                    var consumedTime = (DateTime.UtcNow - lastCheck).Ticks;
                    lastCheck = DateTime.UtcNow;

                    var currentConsumedCpuTIme = Process.GetCurrentProcess().TotalProcessorTime;
                    var diffFromLast = (currentConsumedCpuTIme - lastCpuTime).Ticks;

                    if (diffFromLast == 0)
                    {
                        return 0;
                    }

                    var percentage = (((decimal)diffFromLast) / consumedTime) * 100;

                    lastCpuTime = currentConsumedCpuTIme;
                    return percentage;
                    //var currentCpuTime = Process.GetCurrentProcess().TotalProcessorTime;
                    //var currentCpuPercent = lastCpuTime - currentCpuTime;
                    //lastCpuTime = currentCpuTime;
                    //return Environment.ProcessorCount * currentCpuPercent.TotalSeconds * -1;
                },
                _appLogger
            ));
            ServerInfoListeners.Add(_activatorService.ActivateType<ServerQueuesStatusListener>(publishValue));
        }
        catch (Exception e)
        {
            errors.Add(e.Message);
        }

        if (_cdnFallbackService != null)
        {
            ServerInfoListeners.Add(new StaticDelegateServerInfoListener(publishValue, "CdnCacheStatus", () =>
            {

                var fileStatus = new Dictionary<string, bool>();
                foreach (var cdnResolvedPath in _cdnFallbackService.CdnResolvedPaths)
                {
                    fileStatus.Add(cdnResolvedPath.Key,
                        _localFileProvider.Exists(_cdnFallbackService.ConvertToLocalPath(cdnResolvedPath.Value)));
                }

                return fileStatus;
            }));
        }

        ServerInfoListeners.Add(_activatorService.ActivateType<AppLoggerStatusListener>(publishValue));
        ServerInfoListeners.Add(_activatorService.ActivateType<ServerDrivesListener>(publishValue));

        foreach (var taskTimer in TaskTimerManager.GetFromCache(nameof(ServerInfoManager)))
        {
            taskTimer.Start();
        }

        //foreach (var item in ServerInfoListeners)
        //{
        //	item.PublishValue();
        //}

        Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith((f) =>
        {
            foreach (var item in ServerInfoListeners)
            {
                item.PublishValue();
            }
        });

        KeepAlive = true;
        KeepAliveMonitor();
        return errors;
    }

    public void CloseListeners()
    {
        foreach (var taskTimer in TaskTimerManager.GetFromCache(nameof(ServerInfoManager)))
        {
            taskTimer.Stop();
        }


        while (ServerInfoListeners.TryTake(out var listener))
        {
            listener.Dispose();
        }
    }

    public bool KeepAlive { get; set; }
    public TimeSpan Timeout { get; set; }

    private Task KeepAliveMonitor()
    {
        return Task.Run(async () =>
        {
            while (KeepAlive)
            {
                KeepAlive = false;
                await Task.Delay(Timeout);
            }
            CloseListeners();
        });
    }
}