using System;
namespace MyWorksheet.Website.Server.Models;

public partial class PaymentProviderForRegionMap
{
    public Guid PaymentProviderForRegionMapId { get; set; }

    public Guid IdPaymentProvider { get; set; }

    public Guid RegionId { get; set; }

    public string Comment { get; set; }

    public virtual PaymentProvider IdPaymentProviderNavigation { get; set; }

    public virtual PromisedFeatureRegion Region { get; set; }
}
