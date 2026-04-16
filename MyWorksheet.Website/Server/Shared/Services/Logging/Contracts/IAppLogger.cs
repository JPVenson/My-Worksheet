using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;

public delegate void Log(string message, string category, string level, IDictionary<string, string> optionalData);
public interface IAppLogger : ILoggerDelegation, ICollection<ILoggerDelegation>
{
    ICollection<Func<LoggerEntry, LoggerEntry>> Transform { get; }
    event Log OnLog;
    void LogCritical(string message, string category = null, IDictionary<string, string> optionalData = null);
    void LogError(string message, string category = null, IDictionary<string, string> optionalData = null);
    void LogInformation(string message, string category = null, IDictionary<string, string> optionalData = null);
    void LogVerbose(string message, string category = null, IDictionary<string, string> optionalData = null);
    void LogWarning(string message, string category = null, IDictionary<string, string> optionalData = null);

    new IAppLogger Copy();
}