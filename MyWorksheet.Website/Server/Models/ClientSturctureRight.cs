using System;
namespace MyWorksheet.Website.Server.Models;

public partial class ClientSturctureRight
{
    public Guid ClientSturctureRightId { get; set; }

    public Guid IdRole { get; set; }

    public Guid IdClientStructure { get; set; }

    public bool Inverse { get; set; }

    public virtual ClientStructure IdClientStructureNavigation { get; set; }

    public virtual Role IdRoleNavigation { get; set; }
}
