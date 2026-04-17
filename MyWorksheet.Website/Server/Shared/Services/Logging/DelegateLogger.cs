using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;

namespace MyWorksheet.Website.Server.Shared.Services.Logging;

public class DelegateLogger : ILoggerDelegation, ICollection<ILoggerDelegation>
{
    public DelegateLogger()
    {
        Logger = [];
        Transform = [];
    }

    public ICollection<ILoggerDelegation> Logger { get; set; }
    public ICollection<Func<LoggerEntry, LoggerEntry>> Transform { get; set; }

    public void LogVerbose(string message, string category, IDictionary<string, string> optionalData = null)
    {
        Log(message, category, "Verbose", optionalData);
    }

    public void LogInformation(string message, string category, IDictionary<string, string> optionalData = null)
    {
        Log(message, category, "Information", optionalData);
    }

    public void LogWarning(string message, string category, IDictionary<string, string> optionalData = null)
    {
        Log(message, category, "Warning", optionalData);
    }

    public DelegateLogger CopyLogger()
    {
        var logger = new DelegateLogger();
        foreach (var loggerDelegation in Logger)
        {
            logger.Add(loggerDelegation.Copy());
        }

        logger.Transform = Transform.ToList();
        return logger;
    }

    public void LogError(string message, string category, IDictionary<string, string> optionalData = null)
    {
        Log(message, category, "Error", optionalData);
    }

    public event Log OnLog;

    public void LogCritical(string message, string category, IDictionary<string, string> optionalData = null)
    {
        Log(message, category, "Critical", optionalData);
    }

    public virtual void Log(string message, string category, string level, IDictionary<string, string> optionalData, DateTime dateCreated)
    {
        lock (this)
        {
            optionalData = optionalData ?? new Dictionary<string, string>();
            if (Transform.Count > 0)
            {
                var log = new LoggerEntry(message, category, level, optionalData, dateCreated);
                log = Transform.Aggregate(log, (current, func) => func(current));
                message = log.Message;
                category = log.Category;
                level = log.Level;
                optionalData = log.OptionalData;
                dateCreated = log.DateCreated;
            }

            OnOnLog(message, category, level, optionalData);
            foreach (var loggerDelegation in Logger)
            {
                loggerDelegation.Log(message, category ?? "Unknown", level, optionalData, dateCreated);
            }
        }
    }

    public void Log(string message, string category, string level, IDictionary<string, string> optionalData)
    {
        Log(message, category, level, optionalData, DateTime.UtcNow);
    }

    public void Log(string message, string category, string level)
    {
        Log(message, category, level, null);
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
        foreach (var loggerDelegation in Logger)
        {
            loggerDelegation.Started();
        }
    }

    public ILoggerDelegation Copy()
    {
        return CopyLogger();
    }

    protected virtual void OnOnLog(string message, string category, string level, IDictionary<string, string> optionalData)
    {
        var events = OnLog;
        if (events != null)
        {
            Task.Run(() =>
            {
                events(message, category, level, optionalData);
            });
        }
    }

    public IEnumerator<ILoggerDelegation> GetEnumerator()
    {
        return Logger.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Logger).GetEnumerator();
    }

    public void Add(ILoggerDelegation item)
    {
        Logger.Add(item);
    }

    public void Clear()
    {
        Logger.Clear();
    }

    public bool Contains(ILoggerDelegation item)
    {
        return Logger.Contains(item);
    }

    public void CopyTo(ILoggerDelegation[] array, int arrayIndex)
    {
        Logger.CopyTo(array, arrayIndex);
    }

    public bool Remove(ILoggerDelegation item)
    {
        return Logger.Remove(item);
    }

    public int Count
    {
        get { return Logger.Count; }
    }

    public bool IsReadOnly
    {
        get { return Logger.IsReadOnly; }
    }
}