using System;

namespace MyWorksheet.Website.Server.Models;

public partial class NengineRunningTask : IUserRelation
{
    public Guid NengineRunningTaskId { get; set; }

    public bool IsDone { get; set; }

    public bool IsFaulted { get; set; }

    public string ErrorText { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime? DateRun { get; set; }

    public Guid? IdStoreageEntry { get; set; }

    public string ArgumentsRepresentation { get; set; }

    public Guid IdNengineTemplate { get; set; }

    public Guid? IdProcessor { get; set; }

    public Guid IdCreator { get; set; }

    public bool IsPreview { get; set; }

    public bool IsObsolete { get; set; }

    public virtual AppUser IdCreatorNavigation { get; set; }

    public virtual NengineTemplate IdNengineTemplateNavigation { get; set; }

    public virtual Processor IdProcessorNavigation { get; set; }

    public virtual StorageEntry IdStoreageEntryNavigation { get; set; }
}
