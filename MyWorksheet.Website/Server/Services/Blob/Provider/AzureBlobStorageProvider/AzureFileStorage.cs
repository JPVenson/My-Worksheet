//using System.Security;
//using System.Threading.Tasks;
//using Azure.Storage.Files.Shares;
//using MyWorksheet.Entities.Poco;
//using MyWorksheet.Helper;
//using MyWorksheet.Private.Models.ObjectSchema;
//using MyWorksheet.Public.Models.ObjectSchema;
//using MyWorksheet.Website.Server.Shared.Entities.Manager;
//using MyWorksheet.Website.Server.Shared.ObjectSchema;

//namespace MyWorksheet.Website.Server.Services.Blob.Provider.AzureBlobStorageProvider
//{
//	public class AzureFileStorage : BlobProviderBase
//	{
//		public AzureFileStorage(int storageInstance, StorageProviderData[] data, IDbContextFactory<MyworksheetContext> IDbContextFactory<MyworksheetContext>) : base(storageInstance, data, IDbContextFactory<MyworksheetContext>)
//		{
//		}

//		protected ShareFileClient CreateAccountProvider(Guid appUserId)
//		{
//			var connectionString =
//				$"DefaultEndpointsProtocol=https;AccountName={GetDataFromStore("AccountName")};AccountKey={GetEncryptedDataFromStore("AccountKey", appUserId)};EndpointSuffix=core.windows.net";

//			return new ShareFileClient(connectionString,);
//		}

//		protected CloudFileDirectory CreateBlobContainer(Guid appUserId)
//		{
//			var storageAccount = CreateAccountProvider(appUserId);
//			var cloudBlobClient = storageAccount.CreateCloudFileClient();
//			var storage = cloudBlobClient.GetShareReference(GetDataFromStore("Share"));
//			storage.CreateIfNotExistsAsync().Wait();
//			return storage.GetRootDirectoryReference().GetDirectoryReference(GetDataFromStore("Directory"));
//		}

//		public override async Task<BlobProviderGetResult> GetData(Guid id, Guid appUserId, DbEntities db)
//		{
//			var storageEntry = db.StorageEntries.Find(id);
//			try
//			{
//				var blobReference = CreateBlobContainer(appUserId).GetFileReference(storageEntry.StorageKey);
//				return new BlobProviderGetResult(await blobReference.OpenReadAsync());
//			}
//			catch (StorageException e)
//			{
//				if (e.RequestInformation.HttpStatusCode == 404)
//				{
//					return new BlobProviderGetResult(true);
//				}
//				throw;
//			}
//		}

//		public override async Task<StorageEntry> SetData(BlobData data, Guid appUserId, StorageEntityType entityType, DbEntities db)
//		{
//			var s = await db.Database.RunInTransactionAsync(async d =>
//			{
//				var storageEntry = db.InsertWithSelect(CreateFromBlob(data, entityType, appUserId));
//				var blobReference = CreateBlobContainer(appUserId).GetFileReference(storageEntry.StorageKey);

//				await blobReference.UploadFromStreamAsync(data.DataStream);
//				return storageEntry;
//			});
//			return s;
//		}

//		public override async Task DeleteData(Guid id, Guid appUserId, DbEntities db)
//		{
//			var storageEntry = db.StorageEntries.Find(id);
//			var blobReference = CreateBlobContainer(appUserId).GetFileReference(storageEntry.StorageKey);
//			await blobReference.DeleteAsync();
//		}

//		public override long MaxSizeInBytes { get; protected set; } = long.MaxValue;

//		class AzureBlobArguments : ArgumentsBase
//		{
//			[JsonComment("Storage/AzureBlob.Arguments.Comments.AccountName")]
//			[JsonDisplayKey("Storage/AzureBlob.Arguments.Names.AccountName")]
//			public string AccountName { get; set; }

//			[JsonComment("Storage/AzureBlob.Arguments.Comments.AccountKey")]
//			[JsonDisplayKey("Storage/AzureBlob.Arguments.Names.AccountKey")]
//			public SecureString AccountKey { get; set; }

//			[JsonComment("Storage/AzureFile.Arguments.Comments.Share")]
//			public string Share { get; set; }

//			[JsonComment("Storage/AzureFile.Arguments.Comments.Directory")]
//			[JsonDisplayKey("Storage/AzureFile.Arguments.Names.Directory")]
//			public string Directory { get; set; }
//		}

//		public override IObjectSchema GetSchema()
//		{
//			return JsonSchemaExtensions.JsonSchema(typeof(AzureBlobArguments), IDbContextFactory<MyworksheetContext>.GetDefaultConfig());
//		}
//	}
//}