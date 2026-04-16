using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MyWorksheet.Website.Server.Shared.Services.Logging;

[DebuggerDisplay("{Category} | {Level} | {DateCreated} | {Message}")]
public class LoggerEntry
{
    public LoggerEntry()
    {

    }

    public LoggerEntry(string message, string category, string level, IDictionary<string, string> optionalData, DateTime created)
    {
        Category = category;
        Message = message;
        Level = level;
        OptionalData = optionalData;
        DateCreated = created;
    }

    public string Message { get; private set; }
    public string Category { get; private set; }
    public string Level { get; private set; }
    public IDictionary<string, string> OptionalData { get; private set; }
    public DateTime DateCreated { get; private set; }
}