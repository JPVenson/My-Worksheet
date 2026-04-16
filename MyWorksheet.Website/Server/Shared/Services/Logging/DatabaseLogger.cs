using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MyWorksheet.Webpage.Helper.Utlitiys;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace MyWorksheet.Website.Server.Shared.Services.Logging;

public class DatabaseLogger : ILoggerDelegation
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly CancellationToken _appStopping;

    public DatabaseLogger(IServiceScopeFactory scopeFactory, CancellationToken appStopping)
    {
        _scopeFactory = scopeFactory;
        _appStopping = appStopping;
    }

    public DatabaseLogger(IServiceScopeFactory scopeFactory, CancellationToken appStopping, Func<LoggerEntry, LoggerEntry> transform) : this(scopeFactory, appStopping)
    {
        Transform = transform;
    }

    public Func<LoggerEntry, LoggerEntry> Transform { get; set; }

    public void Log(string message, string category, string level, IDictionary<string, string> optionalData, DateTime dateCreated)
    {
        if (_appStopping.IsCancellationRequested)
        {
            return;
        }

        try
        {
            using var scope = _scopeFactory.CreateScope();
            using var db = scope.ServiceProvider.GetRequiredService<MyworksheetContext>();
            var log = new LoggerEntry(message, category, level, optionalData ?? new Dictionary<string, string>(), dateCreated);
            if (Transform != null)
            {
                log = Transform(log);
            }

            db.Add(new AppLoggerLog()
            {
                Message = log.Message,
                Category = log.Category,
                Level = log.Level,
                AdditionalData = log.OptionalData == null || log.OptionalData.All(e => e.Key == "DatabaseEntryKey") ? null : JsonConvert.SerializeObject(log.OptionalData),
                DateCreated = log.DateCreated,
                DateInserted = DateTime.UtcNow,
                Key = log.OptionalData?.GetOrNull("DatabaseEntryKey")
            });
            db.SaveChanges();
        }
        catch (ObjectDisposedException)
        {
            // Application is shutting down; silently drop the log entry.
        }
    }

    public void Started()
    {
    }

    public ILoggerDelegation Copy()
    {
        return new DatabaseLogger(_scopeFactory, _appStopping, Transform);
    }
}
