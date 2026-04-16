using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class Realm
{
    public Guid RealmId { get; set; }

    public string Named { get; set; }

    public virtual ICollection<Processor> Processors { get; set; } = [];
}
