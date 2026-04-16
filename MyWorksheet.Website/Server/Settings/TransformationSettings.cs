using ServiceLocator.Discovery.Option;

namespace MyWorksheet.Website.Server.Settings;

[FromConfig("transformation")]
public class TransformationSettings
{
    public string State { get; set; }
    public string Realm { get; set; }
    public string Version { get; set; }
}
