namespace MyWorksheet.Website.Server.Services.Blob.Provider.SmbProvider;

public enum ResourceType : int
{
    Any = 0,
    Disk = 1,
    Print = 2,
    Reserved = 8,
}