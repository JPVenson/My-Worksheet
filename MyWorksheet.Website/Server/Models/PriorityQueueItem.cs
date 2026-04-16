using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class PriorityQueueItem : IUserRelation
{
    public Guid PriorityQueueItemId { get; set; }

    public Guid? IdParent { get; set; }

    public Guid IdCreator { get; set; }

    public string ActionKey { get; set; }

    public string DataArguments { get; set; }

    public string Version { get; set; }

    public string Level { get; set; }

    public DateTimeOffset DateOfCreation { get; set; }

    public short DateOfCreationOffset { get; set; }

    public DateTimeOffset? DateOfDone { get; set; }

    public short? DateOfDoneOffset { get; set; }

    public bool Done { get; set; }

    public bool Success { get; set; }

    public string Error { get; set; }

    public virtual AppUser IdCreatorNavigation { get; set; }

    public virtual PriorityQueueItem IdParentNavigation { get; set; }

    public virtual ICollection<PriorityQueueItem> InverseIdParentNavigation { get; set; } = [];
}
