using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class WorksheetAssert : IUserRelation
{
    public Guid WorksheetAssertId { get; set; }

    public string Name { get; set; }

    public decimal Value { get; set; }

    public decimal Tax { get; set; }

    public string Description { get; set; }

    public Guid IdAppUser { get; set; }

    public Guid? IdWorksheet { get; set; }

    public Guid? IdProject { get; set; }

    public virtual AppUser IdAppUserNavigation { get; set; }

    public virtual Project IdProjectNavigation { get; set; }

    public virtual Worksheet IdWorksheetNavigation { get; set; }

    public virtual ICollection<WorksheetAssertsFilesMap> WorksheetAssertsFilesMaps { get; set; } = [];
}
