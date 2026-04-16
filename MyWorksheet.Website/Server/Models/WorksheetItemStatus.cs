using System;

namespace MyWorksheet.Website.Server.Models;

public partial class WorksheetItemStatus : IUserRelation
{
    public Guid WorksheetItemStatusId { get; set; }

    public Guid IdWorksheet { get; set; }

    public Guid IdWorksheetItemStatusLookup { get; set; }

    public Guid IdCreator { get; set; }

    public DateTime DateOfAction { get; set; }

    public virtual AppUser IdCreatorNavigation { get; set; }

    public virtual WorksheetItemStatusLookup IdWorksheetItemStatusLookupNavigation { get; set; }

    public virtual Worksheet IdWorksheetNavigation { get; set; }
}
