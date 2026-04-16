using System;
namespace MyWorksheet.Website.Server.Models;

public partial class WorksheetGeoFenceLocation
{
    public Guid WorksheetGeoFenceId { get; set; }

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public int FenceGroup { get; set; }

    public Guid IdWorksheetGeoFence { get; set; }

    public virtual WorksheetGeoFence IdWorksheetGeoFenceNavigation { get; set; }
}
