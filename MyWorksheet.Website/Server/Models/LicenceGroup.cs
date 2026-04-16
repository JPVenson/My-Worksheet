using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class LicenceGroup
{
    public Guid LicenceGroupId { get; set; }

    public string Descriptor { get; set; }

    public bool Active { get; set; }

    public bool Deleted { get; set; }

    public virtual ICollection<LicenceEntry> LicenceEntries { get; set; } = [];
}
