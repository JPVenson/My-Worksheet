using System;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Webpage.Services.ExecuteLater.Actions;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Blob;
using MyWorksheet.Website.Server.Services.Blob.Thumbnail;
using MyWorksheet.Website.Server.Services.ObjectChanged;
using MyWorksheet.Website.Server.Shared.TaskScheduling;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Services.ScheduledTasks;

[ScheduleOnDemand]
public class ResetThumbnailsTask : BaseTask
{
    private readonly IBlobManagerService _blobManagerService;
    private readonly IThumbnailService _thumbnailService;
    private readonly ObjectChangedService _objectChangedService;

    public ResetThumbnailsTask(IDbContextFactory<MyworksheetContext> dbContextFactory,
        IBlobManagerService blobManagerService,
        IThumbnailService thumbnailService,
        ObjectChangedService objectChangedService) : base(dbContextFactory)
    {
        _blobManagerService = blobManagerService;
        _thumbnailService = thumbnailService;
        _objectChangedService = objectChangedService;
    }

    public override async Task DoWorkAsync(TaskContext context)
    {
        using var db = DbContectFactory.CreateDbContext();
        var logger = context.Logger;

        var storageEntries = db.StorageEntries
            .Where(f => !f.IsDeleted)
            .ToArray();

        logger.LogInformation($"Loaded {storageEntries.Length} entries");

        var thumbnails = storageEntries.Where(e => e.ThumbnailOf.HasValue).ToArray();

        logger.LogInformation($"Of which {thumbnails.Length} are thumbnails");

        foreach (var storageEntry in thumbnails)
        {
            await _blobManagerService.DeleteData(storageEntry.StorageEntryId, storageEntry.IdAppUser);
            db.NengineRunningTasks.Where(e => e.IdStoreageEntry == storageEntry.StorageEntryId).ExecuteDelete();
            db.StorageEntries.Where(e => e.StorageEntryId == storageEntry.StorageEntryId).ExecuteDelete();

            logger.LogInformation($"Deleted thumbnail {storageEntry.FileName}");
        }

        foreach (var storageEntry in storageEntries)
        {
            logger.LogInformation($"Load Storage Data for Entry: {storageEntry.StorageEntryId}: {storageEntry.IdAppUser}->{storageEntry.FileName}");

            var storageData = await _blobManagerService.GetData(storageEntry.StorageEntryId, storageEntry.IdStorageProvider, storageEntry.IdAppUser);
            if (!storageData.Success)
            {
                logger.LogWarning($"Could not load storage entry because: '{storageData.Error}'");
                continue;
            }

            try
            {
                using (storageData.Object)
                {
                    foreach (var thumbSize in _thumbnailService.ThumbnailSizes)
                    {
                        await ExtractThumbnailImage.MakeAndStoreThumbnail(storageData, storageEntry, db,
                            _blobManagerService, _thumbnailService, _objectChangedService, thumbSize.Key,
                            storageEntry.IdAppUser);
                        logger.LogInformation($"Updated Thumbnail for {storageEntry.StorageEntryId} - {thumbSize.Key}");
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogWarning($"Could not set Thumbnail entry because: '{e.Message}'");
            }
        }
    }

    public override string NamedTask { get; protected set; } = "Reevaluate Thumbnails of Storage-Entries";

}