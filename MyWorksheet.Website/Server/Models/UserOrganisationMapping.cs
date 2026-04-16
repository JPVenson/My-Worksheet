using System;

namespace MyWorksheet.Website.Server.Models;

public partial class UserOrganisationMapping
{
    public Guid AppUserId { get; set; }

    public string Username { get; set; }

    public string Email { get; set; }

    public string ContactName { get; set; }

    public bool IsAktive { get; set; }

    public bool MailVerified { get; set; }

    public DateTime? MailVerifiedAt { get; set; }

    public byte MailVerifiedCounter { get; set; }

    public bool AllowUpdates { get; set; }

    public byte[] PasswordHash { get; set; }

    public bool NeedPasswordReset { get; set; }

    public bool AllowFeatureRedeeming { get; set; }

    public bool IsTestUser { get; set; }

    public Guid? IdAddress { get; set; }

    public Guid IdCountry { get; set; }

    public DateTime CreateDate { get; set; }

    public byte[] RowState { get; set; }

    public Guid OrganisationId { get; set; }

    public Organisation Organisation { get; set; }

    public AppUser AppUser { get; set; }
}
