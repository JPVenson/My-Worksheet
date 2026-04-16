using System;

namespace MyWorksheet.Website.Server.Models;

public partial class PerDayReporting
{
    public Guid IdWorksheet { get; set; }

    public virtual Worksheet Worksheet { get; set; }

    public DateTimeOffset DateOfAction { get; set; }

    public int? FromTime { get; set; }

    public int? ToTime { get; set; }

    public Guid? ProjectId { get; set; }

    public virtual Project Project { get; set; }

    public Guid? IdCurrentStatus { get; set; }

    public virtual WorksheetStatusLookup CurrentStatus { get; set; }

    public Guid? IdCreator { get; set; }

    public virtual AppUser Creator { get; set; }

    public int? Timespan { get; set; }

    public int? WorkTimespan { get; set; }

    public string WorksheetActionsCsv { get; set; }
}
