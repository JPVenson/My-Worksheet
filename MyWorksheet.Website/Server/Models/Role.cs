using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class Role
{
    public Guid RoleId { get; set; }

    public string RoleName { get; set; }

    public virtual ICollection<ClientSturctureRight> ClientSturctureRights { get; set; } = [];

    public virtual ICollection<UserRoleMap> UserRoleMaps { get; set; } = [];
}
