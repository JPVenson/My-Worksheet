namespace MyWorksheet.Website.Server.Settings;

public class ServerExternalSettings
{
    public ExternalUserThresholdSettings User { get; set; } = new();
    public UriRulesSettings UriRules { get; set; } = new();
}
