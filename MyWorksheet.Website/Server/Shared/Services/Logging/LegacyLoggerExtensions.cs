using System.Collections.Generic;

namespace Microsoft.Extensions.Logging;

public static class LegacyLoggerExtensions
{
    public static void LogVerbose(this ILogger logger, string message, string category = null, IDictionary<string, string> optionalData = null)
    {
        LogWithLegacyShape(logger, LogLevel.Trace, message, category, optionalData);
    }

    public static void LogInformation(this ILogger logger, string message, string category = null, IDictionary<string, string> optionalData = null)
    {
        LogWithLegacyShape(logger, LogLevel.Information, message, category, optionalData);
    }

    public static void LogWarning(this ILogger logger, string message, string category = null, IDictionary<string, string> optionalData = null)
    {
        LogWithLegacyShape(logger, LogLevel.Warning, message, category, optionalData);
    }

    public static void LogError(this ILogger logger, string message, string category = null, IDictionary<string, string> optionalData = null)
    {
        LogWithLegacyShape(logger, LogLevel.Error, message, category, optionalData);
    }

    public static void LogCritical(this ILogger logger, string message, string category = null, IDictionary<string, string> optionalData = null)
    {
        LogWithLegacyShape(logger, LogLevel.Critical, message, category, optionalData);
    }

    private static void LogWithLegacyShape(ILogger logger, LogLevel level, string message, string category, IDictionary<string, string> optionalData)
    {
        if (logger == null)
        {
            return;
        }

        if (optionalData != null && optionalData.Count > 0)
        {
            using (logger.BeginScope(optionalData))
            {
                logger.Log(level, "[{Category}] {Message}", category ?? "General", message);
            }

            return;
        }

        logger.Log(level, "[{Category}] {Message}", category ?? "General", message);
    }
}
