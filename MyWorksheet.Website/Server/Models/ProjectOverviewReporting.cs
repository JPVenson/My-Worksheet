using System;
namespace MyWorksheet.Website.Server.Models;

public partial class ProjectOverviewReporting
{
    public Guid? IdCreator { get; set; }

    public virtual AppUser Creator { get; set; }

    public Guid? ProjectId { get; set; }

    public virtual Project Project { get; set; }

    public string ProjectName { get; set; }

    public int? WorkedHours { get; set; }

    public decimal? Earned { get; set; }

    public decimal Honorar { get; set; }

    public int UserOrderNo { get; set; }
}
