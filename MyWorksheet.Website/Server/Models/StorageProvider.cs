using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class StorageProvider : IOptionalUserRelation
{
    public Guid StorageProviderId { get; set; }

    public string Name { get; set; }

    public string StorageKey { get; set; }

    public bool IsDefaultProvider { get; set; }

    public Guid? IdAppUser { get; set; }

    public virtual AppUser IdAppUserNavigation { get; set; }

    public virtual ICollection<StorageEntry> StorageEntries { get; set; } = [];

    public virtual ICollection<StorageProviderData> StorageProviderData { get; set; } = [];
}
