using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class Address : IOptionalUserRelation
{
    public Guid AddressId { get; set; }

    public string CompanyName { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Street { get; set; }

    public string StreetNo { get; set; }

    public string ZipCode { get; set; }

    public string City { get; set; }

    public string Country { get; set; }

    public string Phone { get; set; }

    public string EmailAddress { get; set; }

    public DateTime DateOfCreation { get; set; }

    public Guid? IdAppUser { get; set; }

    public Guid? IdOrganisation { get; set; }

    public virtual ICollection<AppUser> AppUsers { get; set; } = [];

    public virtual AppUser IdAppUserNavigation { get; set; }

    public virtual Organisation IdOrganisationNavigation { get; set; }

    public virtual ICollection<Organisation> Organisations { get; set; } = [];

    public virtual ICollection<ProjectAddressMap> ProjectAddressMaps { get; set; } = [];
}

public interface IOptionalUserRelation
{
    // public Guid? IdAppUser { get; set; }    
}

public interface IUserRelation
{
    // public Guid IdAppUser { get; set; }    
}