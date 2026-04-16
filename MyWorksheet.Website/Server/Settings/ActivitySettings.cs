using ServiceLocator.Discovery.Option;

namespace MyWorksheet.Website.Server.Settings;

/// <summary>
/// Settings for activity tracking.
/// Maps to the "activitys" section in appsettings.json (name preserved from the config key).
/// </summary>
[FromConfig("activitys")]
public class ActivitySettings
{
    public TrackerStillRunningSettings TrackerStillRunning { get; set; } = new();
}
