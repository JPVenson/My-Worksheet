using System;
namespace MyWorksheet.Website.Server.Models;

public partial class WorksheetStatusLookupMap
{
    public Guid WorksheetStatusLookupMapId { get; set; }

    public Guid IdFromStatus { get; set; }

    public Guid IdToStatus { get; set; }

    public Guid IdWorkflow { get; set; }

    public virtual WorksheetStatusLookup IdFromStatusNavigation { get; set; }

    public virtual WorksheetStatusLookup IdToStatusNavigation { get; set; }

    public virtual WorksheetWorkflow IdWorkflowNavigation { get; set; }
}
