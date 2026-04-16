namespace MyWorksheet.Website.Server.Settings;

public class ServerFeatureSwitchSettings
{
    public FeatureSwitchToggleSettings Registration { get; set; } = new();
    public FeatureSwitchToggleSettings TestRegistration { get; set; } = new();
}
