using System;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

[ObjectTracking("WorksheetTrack")]
public class WorksheetTimeTrackerViewModel
{
    public Guid WorksheetTrackId { get; set; }
    public Guid IdWorksheet { get; set; }
    public Guid IdProject { get; set; }
    public string Comment { get; set; }
    public Guid IdProjectItemRate { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
}
