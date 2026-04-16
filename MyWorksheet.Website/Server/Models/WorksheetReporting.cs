using System;

namespace MyWorksheet.Website.Server.Models;

public partial class WorksheetReporting
{
    public Guid WorksheetId { get; set; }

    public virtual Worksheet Worksheet { get; set; }

    public DateTimeOffset StartTime { get; set; }

    public DateTimeOffset? EndTime { get; set; }

    public Guid IdProject { get; set; }

    public virtual Project Project { get; set; }

    public bool Hidden { get; set; }

    public string StatusCodeKey { get; set; }

    public Guid IdCreator { get; set; }

    public virtual AppUser Creator { get; set; }
}
