using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class WorksheetStatusLookup
{
    public Guid WorksheetStatusLookupId { get; set; }

    public string DisplayKey { get; set; }

    public string DescriptionKey { get; set; }

    public bool AllowModifications { get; set; }

    public virtual ICollection<WorksheetStatusHistory> WorksheetStatusHistoryIdPostStateNavigations { get; set; } = [];

    public virtual ICollection<WorksheetStatusHistory> WorksheetStatusHistoryIdPreStateNavigations { get; set; } = [];

    public virtual ICollection<WorksheetStatusLookupMap> WorksheetStatusLookupMapIdFromStatusNavigations { get; set; } = [];

    public virtual ICollection<WorksheetStatusLookupMap> WorksheetStatusLookupMapIdToStatusNavigations { get; set; } = [];

    public virtual ICollection<WorksheetWorkflow> WorksheetWorkflowIdDefaultStepNavigations { get; set; } = [];

    public virtual ICollection<WorksheetWorkflow> WorksheetWorkflowIdNoModificationsStepNavigations { get; set; } = [];

    public virtual ICollection<Worksheet> Worksheets { get; set; } = [];
}
