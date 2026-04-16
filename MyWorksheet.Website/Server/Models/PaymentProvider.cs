using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class PaymentProvider
{
    public Guid PaymentProviderId { get; set; }

    public string Name { get; set; }

    public string PaymentKey { get; set; }

    public virtual ICollection<PaymentOrder> PaymentOrders { get; set; } = [];

    public virtual ICollection<PaymentProviderForRegionMap> PaymentProviderForRegionMaps { get; set; } = [];
}
