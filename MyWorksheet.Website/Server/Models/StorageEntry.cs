using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class StorageEntry : IUserRelation
{
    public Guid StorageEntryId { get; set; }

    public string StorageKey { get; set; }

    public string FileName { get; set; }

    public string ContentType { get; set; }

    public Guid? ThumbnailOf { get; set; }

    public bool IsDeleted { get; set; }

    public Guid IdStorageType { get; set; }

    public Guid IdStorageProvider { get; set; }

    public Guid IdAppUser { get; set; }

    public virtual AppUser IdAppUserNavigation { get; set; }

    public virtual StorageProvider IdStorageProviderNavigation { get; set; }

    public virtual StorageType IdStorageTypeNavigation { get; set; }

    public virtual ICollection<StorageEntry> InverseThumbnailOfNavigation { get; set; } = [];

    public virtual ICollection<MailSend> MailSendIdAttachmentNavigations { get; set; } = [];

    public virtual ICollection<MailSend> MailSendIdContentNavigations { get; set; } = [];

    public virtual ICollection<MustachioTemplateFormatter> MustachioTemplateFormatters { get; set; } = [];

    public virtual ICollection<NengineRunningTask> NengineRunningTasks { get; set; } = [];

    public virtual StorageEntry ThumbnailOfNavigation { get; set; }

    public virtual ICollection<WorksheetAssertsFilesMap> WorksheetAssertsFilesMaps { get; set; } = [];

    public virtual ICollection<WorksheetWorkflowStorageMap> WorksheetWorkflowStorageMaps { get; set; } = [];
}
