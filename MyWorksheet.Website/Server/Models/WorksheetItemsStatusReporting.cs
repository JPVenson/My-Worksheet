using System;
namespace MyWorksheet.Website.Server.Models;

public partial class WorksheetItemsStatusReporting
{
    public string Description { get; set; }

    public int? Counts { get; set; }

    public Guid ProjectId { get; set; }

    public virtual Project Project { get; set; }

    public Guid IdCreator { get; set; }

    public virtual AppUser Creator { get; set; }
}
