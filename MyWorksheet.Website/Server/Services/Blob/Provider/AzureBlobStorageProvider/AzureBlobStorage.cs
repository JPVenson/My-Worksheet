using System;
using System.Security;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using MyWorksheet.Helper;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Public.Models.ObjectSchema;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Shared.ObjectSchema;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Services.Blob.Provider.AzureBlobStorageProvider;

public class AzureBlobStorage : BlobProviderBase
{
    class AzureBlobArguments : ArgumentsBase
    {
        [JsonComment("Storage/AzureBlob.Arguments.Comments.AccountName")]
        [JsonDisplayKey("Storage/AzureBlob.Arguments.Names.AccountKey")]
        public string AccountName { get; set; }

        [JsonComment("Storage/AzureBlob.Arguments.Comments.AccountKey")]
        [JsonDisplayKey("Storage/AzureBlob.Arguments.Names.AccountName")]
        public SecureString AccountKey { get; set; }

        [JsonComment("Storage/AzureBlob.Arguments.Comments.Container")]
        [JsonDisplayKey("Storage/AzureBlob.Arguments.Names.Container")]
        public string Container { get; set; }
    }

    public AzureBlobStorage(Guid storageInstance, StorageProviderData[] data, IDbContextFactory<MyworksheetContext> dbContextFactory)
        : base(storageInstance, data, dbContextFactory)
    {
    }

    protected virtual BlobContainerClient CreateAccountProvider(Guid appUserId)
    {
        var connectionString =
            $"DefaultEndpointsProtocol=https;AccountName={GetDataFromStore("AccountName")};AccountKey={GetEncryptedDataFromStore("AccountKey", appUserId)};EndpointSuffix=core.windows.net";
        return new BlobContainerClient(connectionString, GetDataFromStore("Container"));
    }

    protected virtual BlobContainerClient CreateBlobContainer(Guid appUserId)
    {
        var storageAccount = CreateAccountProvider(appUserId);
        storageAccount.CreateIfNotExists();
        return storageAccount;
        //var cloudBlobClient = storageAccount.GetBlobClient("");
        //var storage = cloudBlobClient.GetContainerReference(GetDataFromStore("Container"));
        //storage.CreateIfNotExistsAsync().Wait();
        //return storage;
    }

    public override async Task<BlobProviderGetResult> GetData(Guid id, Guid appUserId, MyworksheetContext db)
    {
        var storageEntry = db.StorageEntries.Find(id);
        try
        {
            var blobReference = CreateBlobContainer(appUserId).GetBlobClient(storageEntry.StorageKey);
            return new BlobProviderGetResult(await blobReference.OpenReadAsync());
        }
        catch (RequestFailedException e)
        {
            if (e.Status == 404)
            {
                return new BlobProviderGetResult(true);
            }
            throw;
        }
    }

    public override async Task<StorageEntry> SetData(BlobData data, Guid appUserId, StorageEntityType entityType, MyworksheetContext db)
    {
        var transaction = await db.Database.BeginTransactionAsync();
        await using (transaction.ConfigureAwait(false))
        {
            var storageEntry = CreateFromBlob(db, data, entityType, appUserId);
            db.Add(storageEntry);
            var blobReference = CreateBlobContainer(appUserId).GetBlobClient(storageEntry.StorageKey);

            var uploadAsync = await blobReference.UploadAsync(data.DataStream);
            if (uploadAsync.GetRawResponse().Status != StatusCodes.Status201Created)
            {
                await transaction.RollbackAsync().ConfigureAwait(false);
                return null;
            }
            await db.SaveChangesAsync().ConfigureAwait(false);
            await transaction.CommitAsync().ConfigureAwait(false);
            return storageEntry;
        }

    }

    public override async Task DeleteData(Guid id, Guid appUserId, MyworksheetContext db)
    {
        var storageEntry = db.StorageEntries.Find(id);
        var blobReference = CreateBlobContainer(appUserId).GetBlobClient(storageEntry.StorageKey);
        await blobReference.DeleteAsync();
    }

    public override long MaxSizeInBytes { get; protected set; } = long.MaxValue;

    public override IObjectSchema GetSchema()
    {
        return JsonSchemaExtensions.JsonSchema(typeof(AzureBlobArguments));
    }
}