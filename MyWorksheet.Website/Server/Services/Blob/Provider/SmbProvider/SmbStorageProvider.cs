//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Security;
//using System.Threading.Tasks;
//using MyWorksheet.Private.Models.ObjectSchema;
//using MyWorksheet.Entities.Poco;
//using MyWorksheet.Helper;
//using MyWorksheet.Services.Blob.Provider.FtpProvider;
//using MyWorksheet.Webpage.Services.Blob.Provider;
//

//namespace MyWorksheet.Services.Blob.Provider.SmbProvider
//{
//	public class SmbStorageProvider : BlobProviderBase
//	{
//		public SmbStorageProvider(int storageInstance, StorageProviderData[] data)
//			: base(storageInstance, data)
//		{
//		}

//		private async Task<NetworkConnection> ConnectToRemoteServer(Guid appUserId)
//		{
//			if (await EnsureServerIsRemote(appUserId) != null)
//			{
//				return null;
//			}
//			var appUser = Db.Select<AppUser>(appUserId);
//			var password = Algorithms.DecryptPassword(GetDataFromStore("Password"), appUser.Username);
//			return new NetworkConnection(GetDataFromStore("ServerAddress"), new NetworkCredential(GetDataFromStore("Username"), password));
//		}

//		private async Task<string> EnsureServerIsRemote(Guid appUserId)
//		{
//			var serverPath = GetDataFromStore("ServerAddress");
//			return await FtpStorageProvider.EnsureDomainIsNotBlacklisted(appUserId, serverPath, "file");
//		}

//		public class NetworkStream : Stream
//		{
//			private readonly FileStream _stream;
//			private readonly NetworkConnection _connection;

//			public NetworkStream(FileStream stream, NetworkConnection connection)
//			{
//				_stream = stream;
//				_connection = connection;
//			}

//			protected override void Dispose(bool disposing)
//			{
//				base.Dispose(disposing);
//				var exceptions = new List<Exception>();
//				try
//				{
//					_stream.Dispose();
//				}
//				catch (Exception e)
//				{
//					exceptions.Add(e);
//				}
//				try
//				{
//					_connection.Dispose();
//				}
//				catch (Exception e)
//				{
//					exceptions.Add(e);
//				}
//				if (exceptions.Any())
//				{
//					throw new AggregateException(exceptions.ToArray());
//				}
//			}

//			public override void Flush()
//			{
//				_stream.Flush();
//			}

//			public override long Seek(long offset, SeekOrigin origin)
//			{
//				return _stream.Seek(offset, origin);
//			}

//			public override void SetLength(long value)
//			{
//				_stream.SetLength(value);
//			}

//			public override int Read(byte[] buffer, int offset, int count)
//			{
//				return _stream.Read(buffer, offset, count);
//			}

//			public override void Write(byte[] buffer, int offset, int count)
//			{
//				_stream.Write(buffer, offset, count);
//			}

//			public override bool CanRead
//			{
//				get { return _stream.CanRead; }
//			}

//			public override bool CanSeek
//			{
//				get { return _stream.CanSeek; }
//			}

//			public override bool CanWrite
//			{
//				get { return _stream.CanWrite; }
//			}

//			public override long Length
//			{
//				get { return _stream.Length; }
//			}

//			public override long Position
//			{
//				get { return _stream.Position; }
//				set { _stream.Position = value; }
//			}
//		}

//		public override async Task<BlobProviderGetResult> GetData(Guid id, Guid appUserId)
//		{
//			if (await EnsureServerIsRemote(appUserId) != null)
//			{
//				return null;
//			}
//			var storageEntry = Db.Select<StorageEntry>(id);
//			try
//			{
//				var fileUrl = new Uri(Path.Combine(GetDataFromStore("ServerAddress"), storageEntry.StorageKey), UriKind.Absolute);
//				var networkConnection = await ConnectToRemoteServer(appUserId);
//				return new BlobProviderGetResult(new NetworkStream(new FileStream(fileUrl.AbsolutePath, FileMode.Open, FileAccess.Read, FileShare.None, 2024,
//					FileOptions.SequentialScan | FileOptions.Asynchronous), networkConnection));
//			}
//			catch (FileNotFoundException)
//			{
//				DeleteStorageItem(storageEntry.StorageEntryId);
//				return new BlobProviderGetResult(true);
//			}
//		}

//		public override async Task<StorageEntry> SetData(BlobData data, Guid appUserId, StorageEntityType entityType)
//		{
//			if (await EnsureServerIsRemote(appUserId) != null)
//			{
//				return null;
//			}

//			using (data)
//			{
//				data.DataStream.Seek(0, SeekOrigin.Begin);

//				StorageEntry storageEntry = null;

//				await Db.Database.RunInTransactionAsync(async (dd) =>
//				{
//					storageEntry = Db.InsertWithSelect(CreateFromBlob(data, entityType, appUserId));
//					var fileUrl = new Uri(Path.Combine(GetDataFromStore("ServerAddress"), storageEntry.StorageKey), UriKind.Absolute);
//					using (ConnectToRemoteServer(appUserId))
//					{
//						using (var sourceStream = new FileStream(fileUrl.AbsolutePath, FileMode.Open, FileAccess.Read, FileShare.None, 2024, FileOptions.SequentialScan | FileOptions.Asynchronous))
//						{
//							await data.DataStream.CopyToAsync(sourceStream);
//							await data.DataStream.FlushAsync();
//							await sourceStream.FlushAsync();
//						}
//					}
//					return "";
//				});

//				await Task.CompletedTask;

//				return storageEntry;
//			}
//		}

//		public override async Task DeleteData(Guid id, Guid appUserId)
//		{
//			if (await EnsureServerIsRemote(appUserId) != null)
//			{
//				return;
//			}

//			var storageEntry = Db.Select<StorageEntry>(id);
//			var fileUrl = new Uri(Path.Combine(GetDataFromStore("ServerAddress"), storageEntry.StorageKey), UriKind.Absolute);
//			using (ConnectToRemoteServer(appUserId))
//			{
//				File.Delete(fileUrl.AbsolutePath);
//			}
//		}

//		public override long MaxSizeInBytes { get; } = int.MaxValue - 10;

//		public override IObjectSchema GetSchema()
//		{
//			return JsonHelper.JsonSchema(new
//			{
//				ServerAddress = "",
//				Username = "",
//				Password = new SecureString()
//			}, Db.Config).ExtendComments(new Dictionary<string, string>()
//			{
//				{"Username","The username(Case Sensitive)" },
//				{"Password","The Password(Case Sensitive, will be stored Encrypted on My-Worksheet)" },
//				{"ServerAddress","The Full quallified URL or IP including the Port starting with file://" },
//			});
//		}
//	}
//}