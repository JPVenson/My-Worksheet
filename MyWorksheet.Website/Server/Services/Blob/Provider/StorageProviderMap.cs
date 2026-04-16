namespace MyWorksheet.Website.Server.Services.Blob.Provider;

public class StorageProviderMap
{
    public StorageProviderMap(string key, string display, StorageProviderFactory storageProviderFactory)
    {
        Key = key;
        Display = display;
        StorageProviderFactory = storageProviderFactory;
    }

    public string Key { get; private set; }
    public string Display { get; private set; }
    public StorageProviderFactory StorageProviderFactory { get; private set; }
}