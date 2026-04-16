using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Website.Server.Services.Blob.Provider;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Storage;

namespace MyWorksheet.Website.Server.Services.Blob;

public interface IBlobManagerService
{
    ICollection<StorageProviderMap> BlobProvidersFactory { get; set; }

    Task DeleteData(Guid id, Guid appUserId);
    Task<BlobManagerGetOperationResult> GetData(Guid id, Guid appUserId);
    Task<BlobManagerGetOperationResult> GetData(Guid id, Guid storageProvider, Guid appUserId);
    Task<StorageProviderStatistics> GetDataStatistics(Guid idStorageProvider, Guid appUserId);
    IObjectSchema GetDataSturcture(string storageKey);
    Task<long> GetMaxReportSize(Guid storageProvider, Guid appUserId, StorageEntityType entityType);
    Task<BlobManagerSetOperationResult> SetData(BlobData data, Guid storageProvider, Guid appUserId,
        StorageEntityType entityType, bool createThumbnail = true);

    Task<BlobManagerGetOperationResult> GetThumbnailAsync(Guid storageEntryId, Guid appUserId, string size);
}