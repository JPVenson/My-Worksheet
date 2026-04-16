using System;
namespace MyWorksheet.Website.Server.Models;

public partial class OrganisationUserMap : IUserRelation
{
    public Guid OrganisationUserMapId { get; set; }

    public Guid IdAppUser { get; set; }

    public Guid IdOrganisation { get; set; }

    public Guid IdRelation { get; set; }

    public virtual AppUser IdAppUserNavigation { get; set; }

    public virtual Organisation IdOrganisationNavigation { get; set; }

    public virtual OrganisationRoleLookup IdRelationNavigation { get; set; }
}
