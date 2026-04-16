namespace MyWorksheet.Website.Server.Settings;

public class FileStorageSettings
{
    public FileTokenStorageSettings Token { get; set; } = new();
    public string Location { get; set; }
    public string Temp { get; set; }
}
