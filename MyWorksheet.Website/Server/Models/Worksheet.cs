using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class Worksheet : IUserRelation
{
    public Guid WorksheetId { get; set; }

    public DateTimeOffset StartTime { get; set; }

    public short StartTimeOffset { get; set; }

    public DateTimeOffset? EndTime { get; set; }

    public short? EndTimeOffset { get; set; }

    public string No { get; set; }

    public string ServiceDescription { get; set; }

    public DateTimeOffset? InvoiceDueDate { get; set; }

    public short? InvoiceDueDateOffset { get; set; }

    public bool Hidden { get; set; }

    public string NumberRangeEntry { get; set; }

    public Guid? IdCurrentStatus { get; set; }

    public Guid IdProject { get; set; }

    public Guid IdCreator { get; set; }

    public Guid? IdWorksheetWorkflow { get; set; }

    public Guid? IdWorksheetWorkflowDataMap { get; set; }

    public virtual AppUser IdCreatorNavigation { get; set; }

    public virtual WorksheetStatusLookup IdCurrentStatusNavigation { get; set; }

    public virtual Project IdProjectNavigation { get; set; }

    public virtual WorksheetWorkflowDataMap IdWorksheetWorkflowDataMapNavigation { get; set; }

    public virtual WorksheetWorkflow IdWorksheetWorkflowNavigation { get; set; }

    public virtual ICollection<WorksheetAssert> WorksheetAsserts { get; set; } = [];

    public virtual ICollection<WorksheetItemStatus> WorksheetItemStatuses { get; set; } = [];

    public virtual ICollection<WorksheetItem> WorksheetItems { get; set; } = [];

    public virtual ICollection<WorksheetStatusHistory> WorksheetStatusHistories { get; set; } = [];

    public virtual ICollection<WorksheetTrack> WorksheetTracks { get; set; } = [];
}
