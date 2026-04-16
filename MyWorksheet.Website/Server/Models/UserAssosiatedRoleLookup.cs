using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class UserAssosiatedRoleLookup
{
    public Guid UserAssosiatedRoleLookupId { get; set; }

    public string Code { get; set; }

    public string Description { get; set; }

    public virtual ICollection<AssosiationInvitation> AssosiationInvitations { get; set; } = [];

    public virtual ICollection<UserAssoisiatedUserMap> UserAssoisiatedUserMaps { get; set; } = [];
}
