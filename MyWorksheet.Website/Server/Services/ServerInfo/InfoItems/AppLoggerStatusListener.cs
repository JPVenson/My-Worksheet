using System;
using System.Collections.Generic;
using MyWorksheet.Website.Server.Shared.Services.Logging;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;

namespace MyWorksheet.Website.Server.Services.ServerInfo.InfoItems;

public class AppLoggerStatusListener : ServerInfoListener
{
    private readonly DelegateLogger _appLogger;

    public AppLoggerStatusListener(Action<string, object> publisher,
        DelegateLogger appLogger) : base(publisher)
    {
        _appLogger = appLogger;
        Key = "ErrorAppLogger";
        AttachedLogger = [];
        AttachedLogger.OnLog += AttachedLogger_OnLog;
        _appLogger.Add(AttachedLogger);
        Entries = [];
    }

    private void AttachedLogger_OnLog(string message, string category, string level, IDictionary<string, string> optionaldata)
    {
        if (level.Equals("ERROR", StringComparison.InvariantCultureIgnoreCase) || level.Equals("Critical", StringComparison.InvariantCultureIgnoreCase))
        {
            var log = new LoggerEntry(message, category, level, optionaldata, DateTime.UtcNow);
            Entries.Add(log);
            Publish(log);
        }
    }

    public DelegateLogger AttachedLogger { get; set; }

    public ICollection<LoggerEntry> Entries { get; set; }

    public override string Key { get; }

    public override void PublishValue()
    {
        foreach (var loggerEntry in Entries)
        {
            Publish(loggerEntry);
        }
    }

    protected override void OnDispose()
    {
        _appLogger.Remove(AttachedLogger);
    }
}