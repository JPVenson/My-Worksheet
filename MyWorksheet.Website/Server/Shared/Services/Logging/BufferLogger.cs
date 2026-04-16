using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;

namespace MyWorksheet.Website.Server.Shared.Services.Logging;

public class BufferLogger : ILoggerDelegation
{
    public BufferLogger()
    {
        LoggerEntries = new ConcurrentQueue<LoggerEntry>();
    }

    public ConcurrentQueue<LoggerEntry> LoggerEntries { get; private set; }

    public void Log(string message, string category, string level, IDictionary<string, string> optionalData, DateTime created)
    {
        LoggerEntries.Enqueue(new LoggerEntry(message, category, level, optionalData, created));
    }

    public void ReplacedWith(ILoggerDelegation logger)
    {
        LoggerEntry logg;
        while (LoggerEntries.TryDequeue(out logg))
        {
            logger.Log(logg.Message, logg.Category, logg.Level, logg.OptionalData, logg.DateCreated);
        }
    }

    public void Started()
    {
    }

    public ILoggerDelegation Copy()
    {
        return new BufferLogger() { LoggerEntries = new ConcurrentQueue<LoggerEntry>(LoggerEntries) };
    }

    public void Log(string message, string category, string level, IDictionary<string, string> optionalData)
    {
        Log(message, category, level, optionalData, DateTime.UtcNow);
    }

    public void Log(string message, string category, string level)
    {
        Log(message, category, level, new Dictionary<string, string>());
    }

    public void Log(string message, string category)
    {
        Log(message, category, null);
    }

    public void Log(string message)
    {
        Log(message, null);
    }
}