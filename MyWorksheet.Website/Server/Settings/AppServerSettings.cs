using ServiceLocator.Discovery.Option;

namespace MyWorksheet.Website.Server.Settings;

[FromConfig("server")]
public class AppServerSettings
{
    public ServerStorageSettings Storage { get; set; } = new();
    public ServerUserSettings User { get; set; } = new();
    public ServerExternalSettings External { get; set; } = new();
    public ServerFeatureSwitchSettings FeatureSwitch { get; set; } = new();
    public ServerRealmSettings Realm { get; set; } = new();
}
