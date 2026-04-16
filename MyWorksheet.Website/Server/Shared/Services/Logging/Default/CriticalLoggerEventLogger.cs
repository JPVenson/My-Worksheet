using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;

namespace MyWorksheet.Website.Server.Shared.Services.Logging.Default;

public class CriticalLoggerEventLogger : ILoggerDelegation
{
    private readonly Action<LoggerEntry> _newEntry;
    private readonly string[] _bufferLevels;

    public CriticalLoggerEventLogger(params string[] bufferLevels) : this((d) => { }, bufferLevels)
    {

    }

    public CriticalLoggerEventLogger(Action<LoggerEntry> newEntry, params string[] bufferLevels)
    {
        _newEntry = newEntry;
        _bufferLevels = bufferLevels;
        CriticalLoggerEntries = [];
    }
    public ConcurrentBag<LoggerEntry> CriticalLoggerEntries { get; set; }

    public void Log(string message, string category, string level, IDictionary<string, string> optionalData, DateTime dateCreated)
    {
        if (_bufferLevels.Any(e => e.Equals(level, StringComparison.OrdinalIgnoreCase)))
        {
            var next = new LoggerEntry(message, category, level, optionalData, dateCreated);
            CriticalLoggerEntries.Add(next);
            Task.Run(() => _newEntry(next));
        }
    }

    public void Log(string message, string category, string level, IDictionary<string, string> optionalData)
    {
        Log(message, category, level, optionalData, DateTime.UtcNow);
    }

    public void Log(string message, string category, string level)
    {
        Log(message, category, level, new Dictionary<string, string>(), DateTime.UtcNow);
    }

    public void Log(string message, string category)
    {
        Log(message, category, null);
    }

    public void Log(string message)
    {
        Log(message, null);
    }

    public void Started()
    {
    }

    public ILoggerDelegation Copy()
    {
        return new CriticalLoggerEventLogger(_newEntry, _bufferLevels);
    }
}