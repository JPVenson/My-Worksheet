using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class PromisedFeatureContent
{
    public Guid PromisedFeatureContentId { get; set; }

    public string DescriptionShort { get; set; }

    public string DescriptionLong { get; set; }

    public decimal Price { get; set; }

    public string Comment { get; set; }

    public DateTime? ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public int? LimitTo { get; set; }

    public int? LimitToUser { get; set; }

    public bool IsActive { get; set; }

    public Guid IdPromisedFeatureRegion { get; set; }

    public Guid IdPromisedFeature { get; set; }

    public virtual PromisedFeature IdPromisedFeatureNavigation { get; set; }

    public virtual PromisedFeatureRegion IdPromisedFeatureRegionNavigation { get; set; }

    public virtual ICollection<PaymentOrder> PaymentOrders { get; set; } = [];
}
