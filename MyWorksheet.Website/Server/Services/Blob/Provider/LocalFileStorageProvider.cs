using System;
using System.Threading.Tasks;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Settings;
using MyWorksheet.Website.Server.Services.UserCounter;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace MyWorksheet.Website.Server.Services.Blob.Provider;

public class LocalFileStorageProvider : BlobProviderBase
{
    private readonly IOptions<AppServerSettings> _serverSettings;
    private readonly IUserQuotaService _userQuotaService;

    public override long MaxSizeInBytes { get; protected set; }

    public static async Task Init(AppServerSettings serverSettings)
    {
        Directory.CreateDirectory(serverSettings.Storage.Default.Path);
    }

    public LocalFileStorageProvider(Guid storageInstance,
        StorageProviderData[] data,
        IOptions<AppServerSettings> serverSettings,
        IUserQuotaService userQuotaService,
        IDbContextFactory<MyworksheetContext> dbContextFactory)
        : base(storageInstance, new StorageProviderData[0], dbContextFactory)
    {
        _serverSettings = serverSettings;
        _userQuotaService = userQuotaService;
        MaxSizeInBytes = (long)serverSettings.Value.Storage.Default.MaxFileSize;
    }

    private string GetFilename(StorageEntry storageEntry)
    {
        return Path.Combine(_serverSettings.Value.Storage.Default.Path, storageEntry.StorageKey);
    }

    public override async Task<BlobProviderGetResult> GetData(Guid id, Guid appUserId, MyworksheetContext db)
    {
        var storageEntry = db.StorageEntries.Find(id);

        var storagePath = GetFilename(storageEntry);
        if (!File.Exists(storagePath))
        {
            return new BlobProviderGetResult(true);
        }
        return new(File.OpenRead(storagePath));
    }

    public static string FormatBytes(long bytes)
    {
        string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
        int i;
        double dblSByte = bytes;
        for (i = 0; i < Suffix.Length && bytes >= 1024; i++, bytes /= 1024)
        {
            dblSByte = bytes / 1024.0;
        }

        return String.Format("{0:0.##} {1}", dblSByte, Suffix[i]);
    }

    public override async Task<StorageEntry> SetData(BlobData data, Guid appUserId, StorageEntityType entityType, MyworksheetContext db)
    {
        if (!data.DataStream.CanSeek)
        {
            throw new InvalidOperationException("The Stream cannot be Seeked");
        }
        if (!data.DataStream.CanRead)
        {
            throw new InvalidOperationException("The Stream cannot be Read");
        }
        var maxFileSize = MaxSizeInBytes;

        if (maxFileSize <= data.DataStream.Length)
        {
            //throw new InvalidOperationException($"The file would exceed the data limit of {FormatBytes(maxFileSize)} for Hosted Files");
        }

        if (!(await _userQuotaService.Add(appUserId, data.DataStream.Length, Quotas.LocalFile)))
        {
            throw new InvalidOperationException("The file would exceed the storage quota");
        }

        var storageEntry = CreateFromBlob(db, data, entityType, appUserId);
        db.Add(storageEntry);

        var filename = GetFilename(storageEntry);
        try
        {
            using var fs = File.OpenWrite(filename);
            await data.DataStream.CopyToAsync(fs).ConfigureAwait(false);
        }
        catch (System.Exception)
        {
            if (Path.Exists(filename))
            {
                File.Delete(filename);
            }
            throw;
        }

        await db.SaveChangesAsync().ConfigureAwait(false);
        return storageEntry;
    }

    public override async Task DeleteData(Guid id, Guid appUserId, MyworksheetContext db)
    {
        var storageEntry = db.StorageEntries.Find(id);
        var fileInfo = new FileInfo(GetFilename(storageEntry));
        var sizeOfToDelete = fileInfo.Length;
        if (fileInfo.Exists)
        {
            fileInfo.Delete();
        }
        if (!await _userQuotaService.Subtract(appUserId, sizeOfToDelete, Quotas.LocalFile))
        {
            throw new InvalidOperationException("The Result would exceed the storage quota");
        }
    }

    public override IObjectSchema GetSchema()
    {
        return new JsonSchema();
    }
}