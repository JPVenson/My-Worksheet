using System;

namespace MyWorksheet.Website.Server.Models;

public partial class ProjectShareKey
{
    public Guid ProjectShareKeyId { get; set; }

    public string Key { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public bool AllowPrinting { get; set; }

    public DateTime? ExpiresAfter { get; set; }

    public bool AllowNonSubmitted { get; set; }

    public Guid IdProject { get; set; }

    public virtual Project IdProjectNavigation { get; set; }
}
