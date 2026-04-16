using System;
namespace MyWorksheet.Website.Server.Models;

public partial class WorksheetAssertsFilesMap
{
    public Guid WorksheetAssertsFilesMapId { get; set; }

    public Guid IdWorksheetAssert { get; set; }

    public Guid IdStorageEntry { get; set; }

    public virtual StorageEntry IdStorageEntryNavigation { get; set; }

    public virtual WorksheetAssert IdWorksheetAssertNavigation { get; set; }
}
