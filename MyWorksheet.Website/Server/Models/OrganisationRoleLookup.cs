using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class OrganisationRoleLookup
{
    public Guid OrganisationRoleLookupId { get; set; }

    public string Name { get; set; }

    public virtual ICollection<OrganisationUserMap> OrganisationUserMaps { get; set; } = [];
}
