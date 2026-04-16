using System;
namespace MyWorksheet.Website.Server.Models;

public partial class SubmittedProject
{
    public Guid? ProjectId { get; set; }

    public virtual Project Project { get; set; }

    public int? Wcount { get; set; }

    public Guid? IdCreator { get; set; }

    public virtual AppUser Creator { get; set; }
}
