using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class SettingsGroup : IUserRelation
{
    public Guid SettingsGroupId { get; set; }

    public string Name { get; set; }

    public string Key { get; set; }

    public Guid IdAppUser { get; set; }

    public virtual AppUser IdAppUserNavigation { get; set; }

    public virtual ICollection<SettingsValue> SettingsValues { get; set; } = [];
}
