using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class Processor
{
    public Guid ProcessorId { get; set; }

    public string ExternalIdentity { get; set; }

    public string Role { get; set; }

    public bool Online { get; set; }

    public string IpOrHostname { get; set; }

    public string AuthKey { get; set; }

    public Guid? IdRealm { get; set; }

    public virtual Realm IdRealmNavigation { get; set; }

    public virtual ICollection<NengineRunningTask> NengineRunningTasks { get; set; } = [];

    public virtual ICollection<ProcessorCapability> ProcessorCapabilities { get; set; } = [];
}
