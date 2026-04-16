using ServiceLocator.Discovery.Option;

namespace MyWorksheet.Website.Server.Settings;

/// <summary>
/// Maps to the "Website:Instance" environment-variable-based config section.
/// Set via environment variable Website__Instance__Id.
/// </summary>
[FromConfig("Website:Instance")]
public class WebsiteInstanceSettings
{
    public string Id { get; set; }
}
