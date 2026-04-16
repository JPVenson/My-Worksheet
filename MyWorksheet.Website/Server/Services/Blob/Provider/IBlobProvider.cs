using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Website.Server.Models;

namespace MyWorksheet.Website.Server.Services.Blob.Provider;

public class BlobProviderGetResult
{
    public BlobProviderGetResult(Stream stream)
    {
        Stream = stream;
        Success = true;
    }

    public BlobProviderGetResult(bool deleteAfter)
    {
        DeleteAfter = deleteAfter;
        Success = false;
    }

    public bool Success { get; set; }
    public Stream Stream { get; set; }
    public bool DeleteAfter { get; set; }
}

public interface IBlobProvider
{
    Task<BlobProviderGetResult> GetData(Guid id, Guid appUserId, MyworksheetContext db);
    Task<StorageEntry> SetData(BlobData data, Guid appUserId, StorageEntityType entityType, MyworksheetContext db);
    Task DeleteData(Guid id, Guid appUserId, MyworksheetContext db);
    Task<IEnumerable<string>> Test(Guid appUserId);
    long MaxSizeInBytes { get; }
    IObjectSchema GetSchema();

    string HelpText(Guid appUserId);
}