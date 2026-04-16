using System;
namespace MyWorksheet.Website.Server.Models;

public partial class WorksheetWorkflowStorageMap : IUserRelation
{
    public Guid WorksheetWorkflowStorageMapId { get; set; }

    public Guid IdStorageEntry { get; set; }

    public Guid IdWorksheetStatusHistory { get; set; }

    public Guid IdAppUser { get; set; }

    public virtual AppUser IdAppUserNavigation { get; set; }

    public virtual StorageEntry IdStorageEntryNavigation { get; set; }

    public virtual WorksheetStatusHistory IdWorksheetStatusHistoryNavigation { get; set; }
}
