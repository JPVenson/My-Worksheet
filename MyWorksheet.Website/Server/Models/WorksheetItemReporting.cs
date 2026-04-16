using System;

namespace MyWorksheet.Website.Server.Models;

public partial class WorksheetItemReporting
{
    public Guid WorksheetItemId { get; set; }

    public virtual WorksheetItem WorksheetItem { get; set; }

    public Guid IdWorksheet { get; set; }

    public virtual Worksheet Worksheet { get; set; }

    public DateTimeOffset DateOfAction { get; set; }

    public int FromTime { get; set; }

    public int ToTime { get; set; }

    public bool Hidden { get; set; }

    public string Comment { get; set; }

    public Guid? ProjectId { get; set; }

    public virtual Project Project { get; set; }

    public string StatusCodeKey { get; set; }

    public Guid? IdCreator { get; set; }

    public virtual AppUser Creator { get; set; }

    public int? Timespan { get; set; }

    public string WorksheetActionsCsv { get; set; }
}
