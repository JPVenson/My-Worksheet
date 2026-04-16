using System;
namespace MyWorksheet.Website.Server.Models;

public partial class MailAccountUserMap : IUserRelation
{
    public Guid MailAccountUserMapId { get; set; }

    public Guid IdMailAccount { get; set; }

    public Guid IdAppUser { get; set; }

    public bool CanEdit { get; set; }

    public virtual AppUser IdAppUserNavigation { get; set; }

    public virtual MailAccount IdMailAccountNavigation { get; set; }
}
