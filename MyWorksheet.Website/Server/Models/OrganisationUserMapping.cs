using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class OrganisationUserMapping
{
    public Guid OrganisationId { get; set; }

    public virtual Organisation Organisation { get; set; }

    public Guid IdAddress { get; set; }

    public virtual Address Address { get; set; }

    public string Name { get; set; }

    public string SharedId { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public Guid? IdParentOrganisation { get; set; }

    public virtual Organisation ParentOrganisation { get; set; }

    public Guid IdAppUser { get; set; }

    public virtual AppUser AppUser { get; set; }

    public string Username { get; set; }

    public ICollection<string> IdRelations { get; set; }
}
