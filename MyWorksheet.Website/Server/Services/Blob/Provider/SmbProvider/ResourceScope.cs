namespace MyWorksheet.Website.Server.Services.Blob.Provider.SmbProvider;

public enum ResourceScope : int
{
    Connected = 1,
    GlobalNetwork,
    Remembered,
    Recent,
    Context
};