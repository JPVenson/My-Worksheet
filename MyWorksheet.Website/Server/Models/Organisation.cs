using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class Organisation
{
    public Guid OrganisationId { get; set; }

    public Guid IdAddress { get; set; }

    public string Name { get; set; }

    public string SharedId { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public Guid? IdParentOrganisation { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = [];

    public virtual Address IdAddressNavigation { get; set; }

    public virtual Organisation IdParentOrganisationNavigation { get; set; }

    public virtual ICollection<Organisation> InverseIdParentOrganisationNavigation { get; set; } = [];

    public virtual ICollection<OrganisationUserMap> OrganisationUserMaps { get; set; } = [];

    public virtual ICollection<Project> Projects { get; set; } = [];

    public virtual ICollection<UserWorkload> UserWorkloads { get; set; } = [];

    public virtual ICollection<WorksheetWorkflowDataMap> WorksheetWorkflowDataMaps { get; set; } = [];
}
