using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class ProjectAddressMapLookup
{
    public Guid ProjectAddressMapLookupId { get; set; }

    public string Code { get; set; }

    public string DisplayKey { get; set; }

    public string DescriptionKey { get; set; }

    public virtual ICollection<ProjectAddressMap> ProjectAddressMaps { get; set; } = [];
}
