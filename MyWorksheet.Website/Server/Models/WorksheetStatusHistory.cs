using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class WorksheetStatusHistory : IUserRelation
{
    public Guid WorksheetStatusHistoryId { get; set; }

    public DateTime DateOfAction { get; set; }

    public string Reason { get; set; }

    public string SystemComment { get; set; }

    public Guid IdWorksheet { get; set; }

    public Guid? IdPreState { get; set; }

    public Guid IdPostState { get; set; }

    public Guid IdChangeUser { get; set; }

    public virtual AppUser IdChangeUserNavigation { get; set; }

    public virtual WorksheetStatusLookup IdPostStateNavigation { get; set; }

    public virtual WorksheetStatusLookup IdPreStateNavigation { get; set; }

    public virtual Worksheet IdWorksheetNavigation { get; set; }

    public virtual ICollection<WorksheetWorkflowStorageMap> WorksheetWorkflowStorageMaps { get; set; } = [];
}
