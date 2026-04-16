using System;
namespace MyWorksheet.Website.Server.Models;

public partial class WorksheetGeoFenceWiFi
{
    public Guid WorksheetGeoFenceWiFiId { get; set; }

    public string Name { get; set; }

    public Guid IdWorksheetGeoFence { get; set; }

    public virtual WorksheetGeoFence IdWorksheetGeoFenceNavigation { get; set; }
}
