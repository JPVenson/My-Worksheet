using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class PromisedFeature
{
    public Guid PromisedFeatureId { get; set; }

    public string DisplayKey { get; set; }

    public int? OrderNumber { get; set; }

    public bool IsActive { get; set; }

    public bool ReoccuringFeature { get; set; }

    public bool InclusiveFeature { get; set; }

    public string Comment { get; set; }

    public virtual ICollection<PromisedFeatureContent> PromisedFeatureContents { get; set; } = [];
}
