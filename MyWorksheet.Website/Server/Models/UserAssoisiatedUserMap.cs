using System;
namespace MyWorksheet.Website.Server.Models;

public partial class UserAssoisiatedUserMap
{
    public Guid UserAssoisiatedUserMapId { get; set; }

    public Guid IdParentUser { get; set; }

    public Guid IdChildUser { get; set; }

    public Guid IdUserRelation { get; set; }

    public Guid? IdInvite { get; set; }

    public virtual AppUser IdChildUserNavigation { get; set; }

    public virtual AssosiationInvitation IdInviteNavigation { get; set; }

    public virtual AppUser IdParentUserNavigation { get; set; }

    public virtual UserAssosiatedRoleLookup IdUserRelationNavigation { get; set; }
}
