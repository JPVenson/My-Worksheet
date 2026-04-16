// using System;
// using System.Threading.Tasks;
// using MyWorksheet.Entities.Poco;
// using MyWorksheet.Private.Models.ObjectSchema;

// namespace MyWorksheet.Website.Server.Services.Blob.Provider.OneDriveProvider;

// public class OneDriveStorageProvider : BlobProviderBase
// {
// 	public OneDriveStorageProvider(int storageInstance, StorageProviderData[] data,
// 		IDbContextFactory<MyworksheetContext> IDbContextFactory<MyworksheetContext>) : base(storageInstance, data, IDbContextFactory<MyworksheetContext>)
// 	{
// 	}

// 	public override Task<BlobProviderGetResult> GetData(Guid id, Guid appUserId, DbEntities db)
// 	{
// 		throw new NotImplementedException();
// 	}

// 	public override Task<StorageEntry> SetData(BlobData data, Guid appUserId, StorageEntityType entityType, DbEntities db)
// 	{
// 		throw new NotImplementedException();
// 	}

// 	public override Task DeleteData(Guid id, Guid appUserId, DbEntities db)
// 	{
// 		throw new NotImplementedException();
// 	}

// 	public override long MaxSizeInBytes { get; protected set; } = int.MaxValue;

// 	public override IObjectSchema GetSchema()
// 	{
// 		throw new NotImplementedException();
// 	}
// }