using System;
using System.Collections.Generic;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;

namespace MyWorksheet.Website.Server.Shared.Services.Logging.Default;

public class TempLogger : ILoggerDelegation
{
    private readonly int _bufferSize;
    private readonly Func<LoggerEntry, bool> _checkAssosiation;

    public string Id { get; private set; }

    private CircularBuffer<LoggerEntry> LoggerEntrys { get; set; }

    public DateTime LastPurge { get; private set; }

    public TempLogger(int bufferSize, string id, Func<LoggerEntry, bool> checkAssosiation)
    {
        Id = id;
        _bufferSize = bufferSize;
        _checkAssosiation = checkAssosiation;
        LoggerEntrys = new CircularBuffer<LoggerEntry>(_bufferSize);
    }

    public IList<LoggerEntry> PurgeEntrys()
    {
        LastPurge = DateTime.UtcNow;
        var list = new List<LoggerEntry>();
        LoggerEntry entry = null;
        while ((entry = LoggerEntrys.Peek()) != null)
        {
            list.Add(entry);
        }
        return list;
    }

    public void Log(string message, string category, string level, IDictionary<string, string> optionalData, DateTime dateCreated)
    {
        var messageObject = new LoggerEntry(message, category, level, optionalData, dateCreated);
        if (_checkAssosiation(messageObject))
            LoggerEntrys.Add(messageObject);
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
        return new TempLogger(_bufferSize, Id, _checkAssosiation);
    }
}