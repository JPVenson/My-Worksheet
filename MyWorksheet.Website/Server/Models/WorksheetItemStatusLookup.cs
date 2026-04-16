using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class WorksheetItemStatusLookup : IOptionalUserRelation
{
    public Guid WorksheetItemStatusLookupId { get; set; }

    public string Description { get; set; }

    public string Action { get; set; }

    public string ActionMeta { get; set; }

    public bool IsPersitent { get; set; }

    public bool IsHidden { get; set; }

    public Guid? IdAppUser { get; set; }

    public virtual AppUser IdAppUserNavigation { get; set; }

    public virtual ICollection<WorksheetItemStatus> WorksheetItemStatuses { get; set; } = [];
}
