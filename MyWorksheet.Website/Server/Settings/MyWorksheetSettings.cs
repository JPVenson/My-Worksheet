using ServiceLocator.Discovery.Option;

namespace MyWorksheet.Website.Server.Settings;

/// <summary>
/// Maps to the "myWorksheet" section in appsettings.json.
/// </summary>
[FromConfig("myWorksheet")]
public class MyWorksheetSettings
{
    public MyWorksheetServerSettings Server { get; set; } = new();
}
