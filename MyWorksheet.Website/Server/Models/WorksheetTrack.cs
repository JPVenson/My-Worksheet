using System;

namespace MyWorksheet.Website.Server.Models;

public partial class WorksheetTrack : IUserRelation
{
    public Guid WorksheetTrackId { get; set; }

    public Guid IdWorksheet { get; set; }

    public Guid IdAppUser { get; set; }

    public Guid IdProjectItemRate { get; set; }

    public DateTimeOffset DateStarted { get; set; }

    public short DateStartedOffset { get; set; }

    public string Comment { get; set; }

    public virtual AppUser IdAppUserNavigation { get; set; }

    public virtual ProjectItemRate IdProjectItemRateNavigation { get; set; }

    public virtual Worksheet IdWorksheetNavigation { get; set; }
}
