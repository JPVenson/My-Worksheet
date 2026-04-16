using System;

namespace MyWorksheet.Website.Server.Models;

public partial class AppLoggerLog
{
    public Guid AppLoggerLogId { get; set; }

    public string Category { get; set; }

    public string Level { get; set; }

    public string Message { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateInserted { get; set; }

    public string AdditionalData { get; set; }

    public string Key { get; set; }
}
