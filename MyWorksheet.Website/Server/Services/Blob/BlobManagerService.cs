using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Services.ExecuteLater;
using MyWorksheet.Shared.Services.PriorityQueue;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Blob.Provider;
using MyWorksheet.Website.Server.Services.Blob.Provider.AzureBlobStorageProvider;
using MyWorksheet.Website.Server.Services.Blob.Provider.FtpProvider;
using MyWorksheet.Website.Server.Services.Blob.Provider.SqlProvider;
using MyWorksheet.Website.Server.Services.Blob.Thumbnail;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater;
using MyWorksheet.Website.Server.Settings;
using MyWorksheet.Website.Server.Shared.Services.Logger;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using MyWorksheet.Website.Shared.Services.Activation;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Storage;
using Microsoft.EntityFrameworkCore;
using ServiceLocator.Attributes;
using Microsoft.Extensions.Options;
using MyWorksheet.Website.Shared.Services;

namespace MyWorksheet.Website.Server.Services.Blob;

[SingletonService(typeof(IBlobManagerService))]
public class BlobManagerService : RequireInit, IBlobManagerService
{
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;
    private readonly ILogger<BlobManagerService> _appLogger;
    private readonly IServerPriorityQueueManager _serverPriorityQueueManager;
    private readonly IThumbnailService _thumbnailService;
    private readonly IOptions<AppServerSettings> _serverSettings;
    private readonly ActivatorService _activatorService;

    public BlobManagerService(IDbContextFactory<MyworksheetContext> dbContextFactory,
        ILogger<BlobManagerService> appLogger,
        IServerPriorityQueueManager serverPriorityQueueManager,
        IThumbnailService thumbnailService,
        IOptions<AppServerSettings> serverSettings,
        ActivatorService activatorService)
    {
        _dbContextFactory = dbContextFactory;
        _appLogger = appLogger;
        _serverPriorityQueueManager = serverPriorityQueueManager;
        _thumbnailService = thumbnailService;
        _serverSettings = serverSettings;
        _activatorService = activatorService;
        BlobProvidersFactory =
        [
            new StorageProviderMap("LocalProvider", "My-Worksheet Hosted", (instanceId, data) => _activatorService.ActivateType< LocalFileStorageProvider>(instanceId, data)),
            new StorageProviderMap("Ftp", "FTP", (instanceId, data) => _activatorService.ActivateType<FtpStorageProvider>(instanceId, data)),
            new StorageProviderMap("AzureBlobStorage", "Azure - Blob Storage", (instanceId, data) => _activatorService.ActivateType<AzureBlobStorage>(instanceId, data)),
            //BlobProvidersFactory.Add(new StorageProviderMap("AzureFileStorage", "Azure - File Storage", (instanceId, data) => _activatorService.ActivateType<AzureFileStorage>(instanceId, data)));
            new StorageProviderMap("Http/Https", "Http Storage", (instanceId, data) => _activatorService.ActivateType<HttpBlobStorageProvider>(instanceId, data)),
            new StorageProviderMap("Ms-Sql", "MS-SQL Hosted", (instanceId, data) => _activatorService.ActivateType<SqlStorageProvider>(instanceId, data)),
        ];

        //BlobProvidersFactory.Add(new StorageProviderMap("SMB", "Server Management Block", (instanceId, data) => new SmbStorageProvider(instanceId, data)));
        //BlobProvidersFactory.Add(new StorageProviderMap(RemoteStorageProvider.Key, "Remote Storage Program", (instanceId, data) => new RemoteStorageProvider(instanceId, data)));
    }

    public override async ValueTask InitAsync()
    {
        await LocalFileStorageProvider.Init(_serverSettings.Value);
    }

    public ICollection<StorageProviderMap> BlobProvidersFactory { get; set; }

    private IBlobProvider CreateBlobProvider(Guid id, Guid appUserId)
    {
        using var dbEntities = _dbContextFactory.CreateDbContext();
        var storageProvider = dbEntities.StorageProviders.Find(id);
        if (storageProvider.IdAppUser.HasValue && storageProvider.IdAppUser.Value != appUserId)
        {
            throw new InvalidOperationException("Invalid Id");
        }

        if (storageProvider == null)
        {
            throw new InvalidOperationException("The given Storage Provider does not exist");
        }
        var provider = BlobProvidersFactory.FirstOrDefault(e => e.Key == storageProvider.StorageKey);
        if (provider == null)
        {
            throw new InvalidOperationException("The given Storage Provider does not exist");
        }

        return provider.StorageProviderFactory(storageProvider.StorageProviderId,
            dbEntities.StorageProviderData.Where(f => f.IdStorageProvider == storageProvider.StorageProviderId).ToArray());
    }

    public async Task<BlobManagerGetOperationResult> GetData(Guid id, Guid storageProvider, Guid appUserId)
    {
        try
        {
            var blobProvider = CreateBlobProvider(storageProvider, appUserId);
            using var db = _dbContextFactory.CreateDbContext();
            var sourceStream = await blobProvider.GetData(id, appUserId, db);
            if (sourceStream == null)
            {
                return new BlobManagerGetOperationResult("Could not Access the Remote file");
            }

            if (sourceStream.DeleteAfter && !sourceStream.Success)
            {
                await DeleteData(id, storageProvider, appUserId);
                return new BlobManagerGetOperationResult("Could not Access the Remote file");
            }

            return new BlobManagerGetOperationResult(sourceStream.Stream);
        }
        catch (Exception e)
        {
            _appLogger.LogError("The operation Get has thrown an exception", LoggerCategories.Storage.ToString(), new Dictionary<string, string>()
            {
                {"Id", id.ToString()},
                {"Exception", e.ToString()}
            });
            return new BlobManagerGetOperationResult(e.Message);
        }
    }

    public async Task<BlobManagerGetOperationResult> GetData(Guid id, Guid appUserId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var firstOrDefault = db.StorageEntries.Where(f => f.StorageEntryId == id).FirstOrDefault();
        if (firstOrDefault == null)
        {
            return new BlobManagerGetOperationResult("Storage Entry does not exist");
        }

        if (firstOrDefault.IsDeleted)
        {
            return new BlobManagerGetOperationResult("File was deleted on Remote Storage");
        }

        return await GetData(id, firstOrDefault.IdStorageProvider, appUserId);
    }

    public async Task<BlobManagerGetOperationResult> GetThumbnailAsync(Guid storageEntryId, Guid appUserId, string size)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var thumbnail = db.StorageEntries
            .Where(f => f.ThumbnailOf == storageEntryId)
            .Where(f => f.FileName.StartsWith(size))
            .FirstOrDefault();

        if (thumbnail != null)
        {
            return await GetData(thumbnail.StorageEntryId, appUserId);
        }

        var storageEntry = db.StorageEntries.Find(storageEntryId);

        if (!storageEntry.IsDeleted && storageEntry.ThumbnailOf != storageEntryId)
        {
            await _serverPriorityQueueManager
                .Enqueue(PriorityManagerLevel.Later, ExternalSchedulableTasks.GET_PREVIEW_IMAGE, appUserId,
                    new Dictionary<string, object>()
                    {
                        {"StorageEntryId", storageEntry.StorageEntryId },
                        {"AppUserID", appUserId },
                        {"Size", size}
                    }, "Feature_Thumbnail_WitExtension_" + storageEntry.ContentType);
        }

        return new BlobManagerGetOperationResult(new MemoryStream(_thumbnailService.DefaultThumbnail(size)));
    }

    public async Task<BlobManagerSetOperationResult> SetData(BlobData data, Guid storageProvider, Guid appUserId,
        StorageEntityType entityType, bool createThumbnail = true)
    {
        try
        {
            var blobProvider = CreateBlobProvider(storageProvider, appUserId);
            using var db = _dbContextFactory.CreateDbContext();
            var storageEntry = await blobProvider.SetData(data, appUserId, entityType, db);
            if (storageEntry != null && createThumbnail)
            {
                await _serverPriorityQueueManager.Enqueue(PriorityManagerLevel.Later, ExternalSchedulableTasks.GET_PREVIEW_IMAGE, appUserId, new Dictionary<string, object>()
                {
                    {"StorageEntryId", storageEntry.StorageEntryId },
                    {"AppUserID", appUserId },
                }, "Feature_Thumbnail_WitExtension_" + storageEntry.ContentType);
            }

            return new BlobManagerSetOperationResult(storageEntry);
        }
        catch (Exception e)
        {
            _appLogger.LogError("The operation Set has thrown an exception", LoggerCategories.Storage.ToString(), new Dictionary<string, string>()
            {
                {"Exception", e.ToString()}
            });
            return new BlobManagerSetOperationResult(
                $"The file with the name: {data.Filename} could not be set. Reason: \r\n{e.Message}");
        }
        finally
        {
            data.Dispose();
        }
    }

    public async Task<long> GetMaxReportSize(Guid storageProvider, Guid appUserId, StorageEntityType entityType)
    {
        await Task.CompletedTask;
        try
        {
            var blobProvider = CreateBlobProvider(storageProvider, appUserId);
            return blobProvider.MaxSizeInBytes;
        }
        catch (Exception)
        {
            return -1;
        }
    }

    public async Task DeleteData(Guid id, Guid appUserId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var firstOrDefault = db.StorageEntries.Where(f => f.StorageEntryId == id).FirstOrDefault();
        if (firstOrDefault == null)
        {
            return;
        }

        await DeleteData(id, firstOrDefault.IdStorageProvider, appUserId);
    }

    private async Task DeleteData(Guid id, Guid idStorageProvider, Guid appUserId)
    {
        try
        {
            var blobProvider = CreateBlobProvider(idStorageProvider, appUserId);

            using var db = _dbContextFactory.CreateDbContext();
            var thumbnails = db.StorageEntries
                .Where(f => f.ThumbnailOf == id)
                .ToArray();

            foreach (var storageEntry in thumbnails)
            {
                if (id == storageEntry.StorageEntryId)
                {
                    continue;
                }

                try
                {
                    await blobProvider.DeleteData(storageEntry.StorageEntryId, appUserId, db);
                }
                catch (Exception e)
                {
                    _appLogger.LogError($"Could not delete the thumbnail '{storageEntry.StorageEntryId}' because the external provider could not delete it", LoggerCategories.Storage.ToString(), new Dictionary<string, string>()
                    {
                        {"Id", id.ToString()},
                        {"Exception", e.ToString()},
                    });
                }

                db.StorageEntries.Remove(storageEntry);
            }

            try
            {
                await blobProvider.DeleteData(id, appUserId, db);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                _appLogger.LogError($"Could not delete the thumbnail '{id}' because the external provider could not delete it", LoggerCategories.Storage.ToString(), new Dictionary<string, string>()
                {
                    {"Id", id.ToString()},
                    {"Exception", e.ToString()},
                });
            }

            db.StorageEntries.Where(e => e.StorageEntryId == id)
            .ExecuteUpdate(f => f.SetProperty(e => e.IsDeleted, true));
        }
        catch (Exception e)
        {
            _appLogger.LogError("The operation Delete has thrown an exception", LoggerCategories.Storage.ToString(), new Dictionary<string, string>()
            {
                {"Id", id.ToString()},
                {"Exception", e.ToString()},
            });
            return;
        }
    }

    public async Task<StorageProviderStatistics> GetDataStatistics(Guid idStorageProvider, Guid appUserId)
    {
        try
        {
            var statistics = new StorageProviderStatistics();
            using var db = _dbContextFactory.CreateDbContext();

            var provider =
                db.StorageProviders
                    .Where(e => e.IdAppUser == null || e.IdAppUser == appUserId)
                    .Where(f => f.StorageProviderId == idStorageProvider)
                    .FirstOrDefault();
            if (provider == null)
            {
                return null;
            }

            var knownEntrys = db.StorageEntries.Where(f => f.IdStorageProvider == provider.StorageProviderId)
                .ToArray();
            statistics.FreeSpaceInMb = -1;
            statistics.CreatedFiles = knownEntrys.Length;
            statistics.UsedSpaceInMb = -1;
            await Task.CompletedTask;
            return statistics;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public IObjectSchema GetDataSturcture(string storageKey)
    {
        return this.BlobProvidersFactory.FirstOrDefault(e => e.Key.Equals(storageKey))?.StorageProviderFactory(Guid.Empty, new StorageProviderData[0]).GetSchema();
    }
}