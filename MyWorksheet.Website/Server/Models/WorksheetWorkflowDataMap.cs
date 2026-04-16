using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class WorksheetWorkflowDataMap : IUserRelation
{
    public Guid WorksheetWorkflowDataMapId { get; set; }

    public Guid IdWorksheetWorkflow { get; set; }

    public string GroupKey { get; set; }

    public Guid IdCreator { get; set; }

    public Guid? IdSharedWithOrganisation { get; set; }

    public virtual AppUser IdCreatorNavigation { get; set; }

    public virtual Organisation IdSharedWithOrganisationNavigation { get; set; }

    public virtual WorksheetWorkflow IdWorksheetWorkflowNavigation { get; set; }

    public virtual ICollection<Project> Projects { get; set; } = [];

    public virtual ICollection<WorksheetWorkflowData> WorksheetWorkflowData { get; set; } = [];

    public virtual ICollection<Worksheet> Worksheets { get; set; } = [];
}
