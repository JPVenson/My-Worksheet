using System;

namespace MyWorksheet.Website.Server.Models;

public partial class UserAction : IUserRelation
{
    public Guid UserActionId { get; set; }

    public byte UserActionType { get; set; }

    public DateTime Date { get; set; }

    public byte[] ActionIp { get; set; }

    public string UserAgent { get; set; }

    public string PcName { get; set; }

    public Guid IdUser { get; set; }

    public virtual AppUser IdUserNavigation { get; set; }
}
