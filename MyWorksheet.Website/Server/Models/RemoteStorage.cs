using System;

namespace MyWorksheet.Website.Server.Models;

public partial class RemoteStorage : IUserRelation
{
    public Guid RemoteStorageId { get; set; }

    public Guid IdCreator { get; set; }

    public string AccessKey { get; set; }

    public byte[] Password { get; set; }

    public bool Status { get; set; }

    public DateTime? LastTimeSinceHeatbeat { get; set; }

    public string SessionKey { get; set; }

    public virtual AppUser IdCreatorNavigation { get; set; }
}
