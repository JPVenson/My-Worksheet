using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class PromisedFeatureRegion
{
    public Guid PromisedFeatureRegionId { get; set; }

    public string RegionName { get; set; }

    public string RegionShortName { get; set; }

    public string Currency { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<AppUser> AppUsers { get; set; } = [];

    public virtual ICollection<PaymentProviderForRegionMap> PaymentProviderForRegionMaps { get; set; } = [];

    public virtual ICollection<PromisedFeatureContent> PromisedFeatureContents { get; set; } = [];
}
