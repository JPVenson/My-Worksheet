using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class PaymentInfo : IUserRelation
{
    public Guid PaymentInfoId { get; set; }

    public string PaymentType { get; set; }

    public string PaymentDisclaimer { get; set; }

    public int? PaymentTarget { get; set; }

    public Guid IdAppUser { get; set; }

    public virtual AppUser IdAppUserNavigation { get; set; }

    public virtual ICollection<PaymentInfoContent> PaymentInfoContents { get; set; } = [];

    public virtual ICollection<Project> Projects { get; set; } = [];
}
