using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class ProjectItemRate : IUserRelation
{
    public Guid ProjectItemRateId { get; set; }

    public string Name { get; set; }

    public decimal Rate { get; set; }

    public decimal TaxRate { get; set; }

    public string CurrencyType { get; set; }

    public bool Hidden { get; set; }

    public Guid IdProject { get; set; }

    public Guid IdCreator { get; set; }

    public Guid IdProjectChargeRate { get; set; }

    public virtual AppUser IdCreatorNavigation { get; set; }

    public virtual ProjectChargeRate IdProjectChargeRateNavigation { get; set; }

    public virtual Project IdProjectNavigation { get; set; }

    public virtual ICollection<Project> Projects { get; set; } = [];

    public virtual ICollection<WorksheetItem> WorksheetItems { get; set; } = [];

    public virtual ICollection<WorksheetTrack> WorksheetTracks { get; set; } = [];
}
