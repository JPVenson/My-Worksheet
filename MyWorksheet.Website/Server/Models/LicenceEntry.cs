using System;

namespace MyWorksheet.Website.Server.Models;

public partial class LicenceEntry
{
    public Guid LicenceEntryId { get; set; }

    public string Descriptor { get; set; }

    public string Username { get; set; }

    public DateTime LastUpdated { get; set; }

    public bool Active { get; set; }

    public string Reason { get; set; }

    public DateTime? ValidUntil { get; set; }

    public Guid IdLicenceGroup { get; set; }

    public bool Deleted { get; set; }

    public string ProgramKey { get; set; }

    public string IsUsernameRelevant { get; set; }

    public string IsPcNameRelevant { get; set; }

    public virtual LicenceGroup IdLicenceGroupNavigation { get; set; }
}
