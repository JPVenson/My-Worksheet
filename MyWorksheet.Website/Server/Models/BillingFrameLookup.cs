using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class BillingFrameLookup
{
    public Guid BillingFrameLookupId { get; set; }

    public string DisplayKey { get; set; }

    public string Code { get; set; }

    public virtual ICollection<Project> Projects { get; set; } = [];
}
