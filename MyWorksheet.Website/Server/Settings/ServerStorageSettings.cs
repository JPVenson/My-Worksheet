namespace MyWorksheet.Website.Server.Settings;

public class ServerStorageSettings
{
    public SqlStorageSettings Sql { get; set; } = new();
    public FileStorageSettings File { get; set; } = new();
    public DefaultCloudStorageSettings Default { get; set; } = new();
}
