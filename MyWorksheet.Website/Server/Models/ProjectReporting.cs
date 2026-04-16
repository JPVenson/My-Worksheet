using System;
namespace MyWorksheet.Website.Server.Models;

public partial class ProjectReporting
{
    public Guid ProjectId { get; set; }

    public virtual Project Project { get; set; }

    public string Name { get; set; }

    public bool Hidden { get; set; }

    public Guid IdCreator { get; set; }

    public virtual AppUser Creator { get; set; }

    public int MoreRatesKnown { get; set; }

    public decimal Honorar { get; set; }

    public decimal TaxRate { get; set; }
}
