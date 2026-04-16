using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class AssosiationInvitation : IUserRelation
{
    public Guid AssosiationInvitationId { get; set; }

    public string ExternalId { get; set; }

    public DateTime? ValidUntil { get; set; }

    public bool ValidOnce { get; set; }

    public bool Revoked { get; set; }

    public DateTime? RevokedDate { get; set; }

    public string RevokeReason { get; set; }

    public Guid IdRequestingUser { get; set; }

    public Guid IdUserAssosiatedRoleLookup { get; set; }

    public virtual AppUser IdRequestingUserNavigation { get; set; }

    public virtual UserAssosiatedRoleLookup IdUserAssosiatedRoleLookupNavigation { get; set; }

    public virtual ICollection<UserAssoisiatedUserMap> UserAssoisiatedUserMaps { get; set; } = [];
}
