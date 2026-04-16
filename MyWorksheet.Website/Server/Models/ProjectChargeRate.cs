using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class ProjectChargeRate
{
    public Guid ProjectChargeRateId { get; set; }

    public string DisplayKey { get; set; }

    public string Code { get; set; }

    public virtual ICollection<ProjectItemRate> ProjectItemRates { get; set; } = [];
}
