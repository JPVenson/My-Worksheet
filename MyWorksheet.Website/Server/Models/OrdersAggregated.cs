using System;
namespace MyWorksheet.Website.Server.Models;

public partial class OrdersAggregated
{
    public Guid IdAppUser { get; set; }

    public virtual AppUser AppUser { get; set; }

    public Guid IdPromisedFeatureContent { get; set; }

    public virtual PromisedFeatureContent PromisedFeatureContent { get; set; }

    public int? Ammount { get; set; }
}
