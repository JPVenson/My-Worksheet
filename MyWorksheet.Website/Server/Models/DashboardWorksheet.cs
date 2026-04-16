using System;

namespace MyWorksheet.Website.Server.Models;

public partial class DashboardWorksheet
{
    public string ProjectName { get; set; }

    public Guid WorksheetId { get; set; }

    public virtual Worksheet Worksheet { get; set; }

    public DateTimeOffset StartTime { get; set; }

    public DateTimeOffset? EndTime { get; set; }

    public string No { get; set; }

    public string ServiceDescription { get; set; }

    public DateTimeOffset? InvoiceDueDate { get; set; }

    public bool Hidden { get; set; }

    public string NumberRangeEntry { get; set; }

    public Guid IdCurrentStatus { get; set; }

    public virtual WorksheetStatusLookup CurrentStatus { get; set; }

    public Guid IdProject { get; set; }

    public virtual Project Project { get; set; }

    public Guid IdCreator { get; set; }

    public virtual AppUser Creator { get; set; }

    public Guid? IdWorksheetWorkflow { get; set; }

    public virtual WorksheetWorkflow WorksheetWorkflow { get; set; }

    public Guid? IdWorksheetWorkflowDataMap { get; set; }

    public virtual WorksheetWorkflowDataMap WorksheetWorkflowDataMap { get; set; }

    public bool? HasDaysOpen { get; set; }
}
