using ServiceLocator.Discovery.Option;

namespace MyWorksheet.Website.Server.Settings;

[FromConfig("Cdn")]
public class CdnSettings
{
    public CdnCacheSettings Cache { get; set; } = new();
}
