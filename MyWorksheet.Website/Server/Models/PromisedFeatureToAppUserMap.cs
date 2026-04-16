using System;

namespace MyWorksheet.Website.Server.Models;

public partial class PromisedFeatureToAppUserMap : IUserRelation
{
    public Guid PromisedFeatureToAppUserMapId { get; set; }

    public Guid? IdOrder { get; set; }

    public DateTime ValidUntil { get; set; }

    public DateTime? ValidFrom { get; set; }

    public string Comment { get; set; }

    public Guid IdAppUser { get; set; }

    public Guid IdFeature { get; set; }

    public virtual AppUser IdAppUserNavigation { get; set; }

    public virtual PaymentOrder IdOrderNavigation { get; set; }
}
