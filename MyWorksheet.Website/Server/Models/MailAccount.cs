using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class MailAccount : IUserRelation
{
    public Guid MailAccountId { get; set; }

    public string Name { get; set; }

    public string EmailAddress { get; set; }

    public int Protocol { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public string ServerAddress { get; set; }

    public int ServerPort { get; set; }

    public Guid IdAppUser { get; set; }

    public virtual AppUser IdAppUserNavigation { get; set; }

    public virtual ICollection<MailAccountUserMap> MailAccountUserMaps { get; set; } = [];

    public virtual ICollection<MailSend> MailSends { get; set; } = [];
}
