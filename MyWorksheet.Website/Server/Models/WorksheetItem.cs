using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class WorksheetItem : IUserRelation
{
    public Guid WorksheetItemId { get; set; }

    public Guid IdWorksheet { get; set; }

    public DateTimeOffset DateOfAction { get; set; }

    public short DateOfActionOffset { get; set; }

    public int FromTime { get; set; }

    public int ToTime { get; set; }

    public bool Hidden { get; set; }

    public string Comment { get; set; }

    public Guid IdCreator { get; set; }

    public Guid IdProjectItemRate { get; set; }

    public virtual AppUser IdCreatorNavigation { get; set; }

    public virtual ProjectItemRate IdProjectItemRateNavigation { get; set; }

    public virtual Worksheet IdWorksheetNavigation { get; set; }

    public virtual ICollection<WorksheetItemStatus> WorksheetItemStatus { get; set; }
}
