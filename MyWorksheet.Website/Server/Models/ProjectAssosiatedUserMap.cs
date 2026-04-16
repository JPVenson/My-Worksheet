using System;
namespace MyWorksheet.Website.Server.Models;

public partial class ProjectAssosiatedUserMap : IUserRelation
{
    public Guid ProjectAssosiatedUserId { get; set; }

    public Guid IdAppUser { get; set; }

    public Guid IdProject { get; set; }

    public bool Read { get; set; }

    public bool Write { get; set; }

    public virtual AppUser IdAppUserNavigation { get; set; }

    public virtual Project IdProjectNavigation { get; set; }
}
