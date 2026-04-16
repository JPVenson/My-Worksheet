using System;
namespace MyWorksheet.Website.Server.Models;

public partial class PaymentInfoContent : IUserRelation
{
    public Guid PaymentInfoContentId { get; set; }

    public string FieldName { get; set; }

    public string FieldValue { get; set; }

    public Guid IdPaymentInfo { get; set; }

    public Guid IdAppUser { get; set; }

    public virtual AppUser IdAppUserNavigation { get; set; }

    public virtual PaymentInfo IdPaymentInfoNavigation { get; set; }
}
