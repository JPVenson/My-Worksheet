using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class PaymentOrder : IUserRelation
{
    public Guid OrderId { get; set; }

    public bool IsOrderDone { get; set; }

    public DateTime? OrderResolveDate { get; set; }

    public DateTime OrderCreatedDate { get; set; }

    public bool IsOrderSuccess { get; set; }

    public string OrderError { get; set; }

    public string TransactionInfos { get; set; }

    public string Comment { get; set; }

    public Guid IdAppUser { get; set; }

    public Guid IdPaymentProvider { get; set; }

    public Guid IdPromisedFeatureContent { get; set; }

    public virtual AppUser IdAppUserNavigation { get; set; }

    public virtual PaymentProvider IdPaymentProviderNavigation { get; set; }

    public virtual PromisedFeatureContent IdPromisedFeatureContentNavigation { get; set; }

    public virtual ICollection<PromisedFeatureToAppUserMap> PromisedFeatureToAppUserMaps { get; set; } = [];
}
