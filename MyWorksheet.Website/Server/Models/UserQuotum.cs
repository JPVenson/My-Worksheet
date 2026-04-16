using System;
namespace MyWorksheet.Website.Server.Models;

public partial class UserQuota : IUserRelation
{
    public Guid UserQuotaId { get; set; }

    public Guid IdAppUser { get; set; }

    public bool QuotaUnlimited { get; set; }

    public int QuotaValue { get; set; }

    public int QuotaMax { get; set; }

    public int QuotaMin { get; set; }

    public int QuotaType { get; set; }

    public virtual AppUser IdAppUserNavigation { get; set; }
}
