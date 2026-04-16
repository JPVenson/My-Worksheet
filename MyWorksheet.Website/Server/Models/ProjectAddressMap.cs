using System;
namespace MyWorksheet.Website.Server.Models;

public partial class ProjectAddressMap
{
    public Guid ProjectAddressMapId { get; set; }

    public Guid IdProject { get; set; }

    public Guid IdAddress { get; set; }

    public Guid IdProjectAddressMapLookup { get; set; }

    public virtual Address IdAddressNavigation { get; set; }

    public virtual ProjectAddressMapLookup IdProjectAddressMapLookupNavigation { get; set; }

    public virtual Project IdProjectNavigation { get; set; }
}
