using ServiceLocator.Discovery.Option;

namespace MyWorksheet.Website.Server.Settings;

/// <summary>
/// Settings for maintenance mode templates.
/// Maps to the "maintainace" section in appsettings.json (name is preserved as-is from the config key).
/// </summary>
[FromConfig("maintainace")]
public class MaintenanceAppSettings
{
    public MaintenanceTemplateSettings Templates { get; set; } = new();
}
