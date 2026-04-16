using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class WorksheetWorkflow
{
    public Guid WorksheetWorkflowId { get; set; }

    public string Comment { get; set; }

    public string DisplayKey { get; set; }

    public string ProviderKey { get; set; }

    public bool? NeedsCustomData { get; set; }

    public Guid IdDefaultStep { get; set; }

    public virtual WorksheetStatusLookup IdDefaultStepNavigation { get; set; }

    public virtual ICollection<Project> Projects { get; set; } = [];

    public virtual ICollection<WorksheetStatusLookupMap> WorksheetStatusLookupMaps { get; set; } = [];

    public virtual ICollection<WorksheetWorkflowDataMap> WorksheetWorkflowDataMaps { get; set; } = [];

    public virtual ICollection<Worksheet> Worksheets { get; set; } = [];
}
