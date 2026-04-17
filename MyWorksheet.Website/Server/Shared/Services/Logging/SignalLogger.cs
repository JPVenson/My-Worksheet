using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Util.View;
using MyWorksheet.Website.Server.Shared.Hubs.Hubs;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

namespace MyWorksheet.Website.Server.Shared.Services.Logging;

public class SignalLogger : ILoggerDelegation
{
    private readonly IHubContext<LoggerHub> _loggerHub;
    private readonly ILogger<SignalLogger> _logger;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    public SignalLogger(IHubContext<LoggerHub> loggerHub, ILogger<SignalLogger> logger, IHostApplicationLifetime hostApplicationLifetime)
    {
        _loggerHub = loggerHub;
        _logger = logger;
        _hostApplicationLifetime = hostApplicationLifetime;
        _onLog = new AutoResetEvent(false);
        var bufferA = new List<AppLoggerLogViewModel>();
        var bufferB = new List<AppLoggerLogViewModel>();
        _buffer = bufferA;

        async void Action()
        {
            while (!hostApplicationLifetime.ApplicationStopping.IsCancellationRequested)
            {
                _onLog.WaitOne();
                await Task.Delay(200);
                var items = Interlocked.Exchange(ref _buffer, _buffer == bufferA ? bufferB : bufferA);
                await _loggerHub.Clients.Group("ALL").SendCoreAsync(nameof(ILoggerHub.LogEntries), new object[] { items.ToArray() });
                items.Clear();
            }
        }

        new Task(Action, TaskCreationOptions.LongRunning).Start();
    }

    private IList<AppLoggerLogViewModel> _buffer;
    private AutoResetEvent _onLog;

    public void Log(string message, string category, string level, IDictionary<string, string> optionalData, DateTime dateCreated)
    {
        _buffer.Add(new AppLoggerLogViewModel()
        {
            Level = level,
            Category = category,
            DateCreated = dateCreated,
            Key = optionalData?.GetOrDefault("DatabaseEntryKey", null),
            Message = message,
            OptionalData = optionalData?.Where(e => e.Key != "DatabaseEntryKey").ToDictionary(e => e.Key, e => e.Value)
        });
        _onLog.Set();
    }

    public void Started()
    {
    }

    public ILoggerDelegation Copy()
    {
        return new SignalLogger(_loggerHub, _logger, _hostApplicationLifetime);
    }
}
