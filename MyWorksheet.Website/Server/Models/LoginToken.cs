using System;

namespace MyWorksheet.Website.Server.Models;

public partial class LoginToken : IUserRelation
{
    public Guid LoginTokenId { get; set; }

    public string Token { get; set; }

    public DateTime ValidUntil { get; set; }

    public string RemoteIp { get; set; }

    public Guid IdAppUser { get; set; }

    public virtual AppUser IdAppUserNavigation { get; set; }
}
