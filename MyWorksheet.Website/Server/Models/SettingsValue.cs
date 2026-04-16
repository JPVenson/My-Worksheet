using System;
namespace MyWorksheet.Website.Server.Models;

public partial class SettingsValue : IUserRelation
{
    public Guid SettingsValueId { get; set; }

    public string Value { get; set; }

    public string Name { get; set; }

    public byte[] RowState { get; set; }

    public Guid IdSettingsGroup { get; set; }

    public Guid IdAppUser { get; set; }

    public virtual AppUser IdAppUserNavigation { get; set; }

    public virtual SettingsGroup IdSettingsGroupNavigation { get; set; }
}
