using System;
namespace MyWorksheet.Website.Server.Models;

public partial class WorksheetComment
{
    public Guid IdCreator { get; set; }

    public virtual AppUser Creator { get; set; }

    public string Comment { get; set; }

    public Guid WorksheetId { get; set; }

    public virtual Worksheet Worksheet { get; set; }
}
