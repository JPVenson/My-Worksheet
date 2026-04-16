using System;

namespace MyWorksheet.Website.Server.Models;

public partial class AppNumberRange : IOptionalUserRelation
{
    public Guid AppNumberRangeId { get; set; }

    public string Template { get; set; }

    public long Counter { get; set; }

    public string Code { get; set; }

    public bool IsActive { get; set; }

    public Guid? IdUser { get; set; }

    public virtual AppUser IdUserNavigation { get; set; }
}
