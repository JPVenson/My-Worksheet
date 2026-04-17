namespace MyWorksheet.Website.Server.Settings;

public class ServerUserSettings
{
    public ServerUserCreateSettings Create { get; set; } = new();

    public ServerUserDefaultSettings Default { get; set; } = new();
}
