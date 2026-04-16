using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Helper;
using MyWorksheet.Services.ExecuteLater;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Blob;
using MyWorksheet.Website.Server.Services.Blob.Provider;
using MyWorksheet.Website.Server.Services.Blob.Thumbnail;
using MyWorksheet.Website.Server.Services.ExecuteLater.Actions;
using MyWorksheet.Website.Server.Services.ObjectChanged;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater.ArgumentStore;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater.Contracts;
using MyWorksheet.Website.Shared.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Webpage.Services.ExecuteLater.Actions;

[PriorityQueueItem(ActionKey)]
public class ExtractThumbnailImage : IPriorityQueueAction
{
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;
    private readonly IBlobManagerService _blobManagerService;
    private readonly IThumbnailService _thumbnailService;
    private readonly ObjectChangedService _objectChangedService;

    public ExtractThumbnailImage(IDbContextFactory<MyworksheetContext> dbContextFactory, IBlobManagerService blobManagerService,
        IThumbnailService thumbnailService, ObjectChangedService objectChangedService)
    {
        _dbContextFactory = dbContextFactory;
        _blobManagerService = blobManagerService;
        _thumbnailService = thumbnailService;
        _objectChangedService = objectChangedService;
    }

    class Arguments : ArgumentsBase
    {
        public Arguments()
        {

        }

        public static Arguments Parse(IDictionary<string, object> arguments)
        {
            var args = new Arguments();
            args.SetOrAbort(arguments, nameof(Size));
            args.SetOrAbort(arguments, nameof(AppUserId));
            args.SetOrAbort(arguments, nameof(StorageEntryId));

            return args.GetIfValid() as Arguments;
        }

        public string Size { get; private set; }
        public Guid AppUserId { get; private set; }
        public Guid StorageEntryId { get; private set; }
    }

    public const string ActionKey = ExternalSchedulableTasks.GET_PREVIEW_IMAGE;
    public string Name => "Extracts an Preview Image from an File";
    public string Key => ActionKey;
    public Version Version { get; set; }

    /// <inheritdoc />
    public bool ValidateArguments(IDictionary<string, object> arguments)
    {
        return new DictionaryElementsValidator<string, object>(arguments)
            .ContainsKey("StorageEntryId")
            .Result;
    }

    public async Task Execute(PriorityQueueElement queueElement)
    {
        var arguments = Arguments.Parse(queueElement.Arguments);
        if (arguments == null)
        {
            return;
        }

        var appUserId = queueElement.UserId;
        var storageProvider = _blobManagerService;

        var db = _dbContextFactory.CreateDbContext();

        var storageEntry = db.StorageEntries.Find(arguments.StorageEntryId);

        var fileData = await storageProvider.GetData(arguments.StorageEntryId, appUserId);
        if (!fileData.Success)
        {
            return;
        }

        var firstOrDefault = db.StorageEntries
            .Where(f => f.ThumbnailOf == arguments.StorageEntryId)
            .Where(f => f.FileName.StartsWith(arguments.Size))
            .FirstOrDefault();

        if (firstOrDefault != null)
        {
            return;
        }

        await MakeAndStoreThumbnail(fileData, storageEntry, db, storageProvider, _thumbnailService, _objectChangedService, arguments.Size, appUserId);
        //AccessElement<StorageEntryChangedHubInfo>.Instance.SendChanged(storageEntry.StorageEntryId,
        //(int)appUserId);
    }

    public static async Task MakeAndStoreThumbnail(BlobManagerGetOperationResult fileData,
        StorageEntry storageEntry,
        MyworksheetContext dbEntities,
        IBlobManagerService storageProvider,
        IThumbnailService thumbnailService,
        ObjectChangedService objectChangedService,
        string size,
        Guid appUserId)
    {
        byte[] thumbnailData;
        using (fileData.Object)
        {
            thumbnailData =
                await thumbnailService.CreateThumbnailFromFileDetailed(fileData.Object,
                    storageEntry.FileName,
                    storageEntry.ContentType,
                    size);
            if (thumbnailData == null)
            {
                dbEntities.StorageEntries
                .Where(e => e.StorageEntryId == storageEntry.StorageEntryId)
                .ExecuteUpdate(f => f.SetProperty(e => e.ThumbnailOf, storageEntry.StorageEntryId));
                //create self reference if there is no thumbnail available
                return;
            }
        }

        BlobManagerSetOperationResult blobManagerSetOperationResult;
        using (var thumbnailDataStream = new MemoryStream(thumbnailData))
        {
            blobManagerSetOperationResult = await storageProvider.SetData(
                new BlobData(thumbnailDataStream, size + "_" + storageEntry.FileName),
                storageEntry.IdStorageProvider, storageEntry.IdAppUser, StorageEntityType.Thumbnail, false);
        }

        if (!blobManagerSetOperationResult.Success)
        {
            return;
        }

        dbEntities.StorageEntries
            .Where(e => e.StorageEntryId == storageEntry.StorageEntryId)
            .ExecuteUpdate(f => f.SetProperty(e => e.ThumbnailOf, blobManagerSetOperationResult.Object.StorageEntryId));


        await objectChangedService.SendObjectChanged(ChangeEventTypes.Changed, "StorageEntry", storageEntry.StorageEntryId, null, appUserId);
    }
}