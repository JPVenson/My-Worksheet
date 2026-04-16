using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class WorksheetGeoFence : IUserRelation
{
    public Guid WorksheetGeoFenceId { get; set; }

    public string Name { get; set; }

    public Guid IdProject { get; set; }

    public bool IsEnabled { get; set; }

    public Guid IdAppUser { get; set; }

    public virtual AppUser IdAppUserNavigation { get; set; }

    public virtual ICollection<WorksheetGeoFenceLocation> WorksheetGeoFenceLocations { get; set; } = [];

    public virtual ICollection<WorksheetGeoFenceWiFi> WorksheetGeoFenceWiFis { get; set; } = [];
}
