using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Website.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Services.Blob.Provider.SqlProvider;

public class SqlStorageProvider : BlobProviderBase
{
    public SqlStorageProvider(Guid storageInstance, StorageProviderData[] data, IDbContextFactory<MyworksheetContext> dbContextFactory)
        : base(storageInstance, data, dbContextFactory)
    {
    }

    public override async Task<BlobProviderGetResult> GetData(Guid id, Guid appUserId, MyworksheetContext db)
    {
        var storageEntry = await db.StorageEntries.FindAsync(id);
        if (storageEntry == null || !Guid.TryParse(storageEntry.StorageKey, out var key))
        {
            return new BlobProviderGetResult(true);
        }

        var blob = await db.HostedStorageBlobs.FirstOrDefaultAsync(e => e.Key == key);
        if (blob == null)
        {
            return new BlobProviderGetResult(true);
        }

        return new BlobProviderGetResult(new MemoryStream(blob.Value));
    }

    public override async Task<StorageEntry> SetData(BlobData data, Guid appUserId, StorageEntityType entityType, MyworksheetContext db)
    {
        using var ms = new MemoryStream();
        await data.DataStream.CopyToAsync(ms);

        var key = Guid.NewGuid();
        var blob = new HostedStorageBlob
        {
            Key = key,
            Value = ms.ToArray()
        };
        db.Add(blob);

        var storageEntry = CreateFromBlob(db, data, entityType, appUserId);
        storageEntry.StorageKey = key.ToString();
        db.Add(storageEntry);
        await db.SaveChangesAsync();
        return storageEntry;
    }

    public override async Task DeleteData(Guid id, Guid appUserId, MyworksheetContext db)
    {
        var storageEntry = await db.StorageEntries.FindAsync(id);
        if (storageEntry != null && Guid.TryParse(storageEntry.StorageKey, out var key))
        {
            await db.HostedStorageBlobs.Where(e => e.Key == key).ExecuteDeleteAsync();
        }
    }

    public override long MaxSizeInBytes { get; protected set; } = 50 * 1024 * 1024;

    public override IObjectSchema GetSchema()
    {
        return new JsonSchema();
    }
}