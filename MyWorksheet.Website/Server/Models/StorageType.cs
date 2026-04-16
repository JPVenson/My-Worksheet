using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class StorageType
{
    public Guid StorageTypeId { get; set; }

    public string Name { get; set; }

    public virtual ICollection<StorageEntry> StorageEntries { get; set; } = [];
}
