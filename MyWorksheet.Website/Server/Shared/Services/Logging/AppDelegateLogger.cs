using System;
using System.Collections.Generic;
using System.Linq;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using Microsoft.Extensions.Logging;

namespace MyWorksheet.Website.Server.Shared.Services.Logging;

public class AppDelegateLogger : ILoggerDelegation
{
    private readonly ILogger _logger;

    public AppDelegateLogger(ILogger logger)
    {
        _logger = logger;
    }

    public void Log(string message, string category, string level, IDictionary<string, string> optionalData, DateTime dateCreated)
    {
        LogLevel logLevel;
        if (!Enum.TryParse<LogLevel>(level, out logLevel))
        {
            logLevel = LogLevel.Trace;
        }

        if (optionalData != null && optionalData.Any())
        {
            message += Environment.NewLine;
            foreach (var data in optionalData)
            {
                message += data.Key + ": " + data.Value;
                message += Environment.NewLine;
            }
        }
        _logger.Log(logLevel, new EventId(category.GetHashCode(), category), message);
    }

    public void Started()
    {
    }

    public ILoggerDelegation Copy()
    {
        return new AppDelegateLogger(_logger);
    }
}
