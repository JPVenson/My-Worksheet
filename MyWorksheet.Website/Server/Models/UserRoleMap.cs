using System;
namespace MyWorksheet.Website.Server.Models;

public partial class UserRoleMap : IUserRelation
{
    public Guid UserRoleMapId { get; set; }

    public Guid IdUser { get; set; }

    public Guid IdRole { get; set; }

    public virtual Role IdRoleNavigation { get; set; }

    public virtual AppUser IdUserNavigation { get; set; }
}
