using ServiceLocator.Discovery.Option;

namespace MyWorksheet.Website.Server.Settings;

/// <summary>
/// Maps to the "is:it" section in appsettings.json.
/// </summary>
[FromConfig("is:it")]
public class IsItSettings
{
    public bool Free { get; set; }
}
