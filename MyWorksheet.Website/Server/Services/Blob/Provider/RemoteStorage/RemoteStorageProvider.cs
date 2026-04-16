//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Web;
//using Katana.CommonTasks.Middelware.File.Handler.Cache;
//using Katana.CommonTasks.Unity;
//using MyWorksheet.Private.Models.ObjectSchema;
//using MyWorksheet.Public.Models.ViewModel.RemoteStorage;
//using MyWorksheet.Webpage.Entities.Manager;
//using MyWorksheet.Webpage.Entities.Poco;
//using MyWorksheet.Webpage.Hubs.HubInfos;

//namespace MyWorksheet.Webpage.Services.Blob.Provider.RemoteStorage
//{
//	public class RemoteStorageService
//	{
//		public void StorageStatusChange(bool online, string accessKey)
//		{
//			var dbEntities = IoC.Resolve<DbEntities>();
//			dbEntities.Query().Update.Table<RemoteStorage>()
//				.Set
//				.Column(f => f.Status)
//				.Value(online)
//				.Where
//				.Column(f => f.AccessKey)
//				.Is.EqualsTo(accessKey)
//				.ExecuteNonQuery();
//		}

//		public ConcurrentDictionary<string, RemoteStorageMiddelware> RemoteStorageMiddelwares { get; private set; }

//		public async Task SaveToConnector(Stream dataSource, string accessKey)
//		{
//			RemoteStorageMiddelware middelware = null;
//			var operationId = Guid.NewGuid().ToString("N");
//			RemoteStorageMiddelwares.TryAdd(operationId, middelware = new RemoteStorageMiddelware()
//			{
//				Content = dataSource,
//				AccessKey = accessKey
//			});
//			AccessElement<RemoteStorageHubInfo>.Instance.SendCommand(Commands.GetContentFromMe, accessKey, operationId);
//			await middelware.Done.WaitHandle.WaitOneAsync();
//		}

//		public async Task GetFromConnector(Stream target, string accessKey)
//		{
//			RemoteStorageMiddelware middelware = null;
//			var operationId = Guid.NewGuid().ToString("N");
//			RemoteStorageMiddelwares.TryAdd(operationId, middelware = new RemoteStorageMiddelware()
//			{
//				Content = target,
//				AccessKey = accessKey
//			});
//			AccessElement<RemoteStorageHubInfo>.Instance.SendCommand(Commands.PushContentToMe, accessKey, operationId);
//			await middelware.Done.WaitHandle.WaitOneAsync();
//		}

//		public async Task PullContentForStorage(string accessKey, string operationId, Stream targetStream)
//		{
//			if (!RemoteStorageMiddelwares.ContainsKey(operationId))
//			{
//				return;
//			}

//			var middelware = RemoteStorageMiddelwares[operationId];
//			await middelware.Content.CopyToAsync(targetStream);
//			middelware.Content.Dispose();
//			middelware.Done.Set();
//		}
//	}

//	public class RemoteStorageMiddelware
//	{
//		private readonly CancellationTokenSource _timeoutSource;
//		public RemoteStorageMiddelware()
//		{
//			_timeoutSource = new CancellationTokenSource();
//			_timeoutSource.CancelAfter(TimeSpan.FromSeconds(30));
//			Timeout = _timeoutSource.Token;
//			Timeout.Register(() => Done.Set());

//			Done = new ManualResetEventSlim();
//		}

//		public string AccessKey { get; set; }
//		public Stream Content { get; set; }
//		public CancellationToken Timeout { get; set; }
//		public ManualResetEventSlim Done { get; set; }
//	}

//	public class RemoteStorageProvider : BlobProviderBase
//	{
//		public static string Key { get; private set; } = nameof(RemoteStorageProvider);

//		public RemoteStorageProvider(int storageInstance, StorageProviderData[] data) : base(storageInstance, data)
//		{
//		}

//		public override Task<BlobProviderGetResult> GetData(Guid id, Guid appUserId)
//		{
//			throw new NotImplementedException();
//		}

//		public override Task<StorageEntry> SetData(BlobData data, Guid appUserId, StorageEntityType entityType)
//		{
//			throw new NotImplementedException();
//		}

//		public override Task DeleteData(Guid id, Guid appUserId)
//		{
//			throw new NotImplementedException();
//		}

//		public override long MaxSizeInBytes { get; }

//		public override IObjectSchema GetSchema()
//		{
//			throw new NotImplementedException();
//		}
//	}
//}