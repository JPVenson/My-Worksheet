namespace MyWorksheet.Webpage.Helper.Utlitiys;

//public class MultipartStreamDataProvider : MultipartStreamProvider, IDisposable
//{
//	private readonly string _operationKey;
//	private readonly Guid _userId;

//	public MultipartStreamDataProvider(string operationKey, Guid userId)
//	{
//		_operationKey = operationKey;
//		_userId = userId;
//		FileData = new ConcurrentDictionary<MultipartFileData, FileStream>();
//	}

//	public IDictionary<MultipartFileData, FileStream> FileData { get; private set; }

//	/// <inheritdoc />
//	public override Stream GetStream(HttpContent parent, HttpContentHeaders headers)
//	{
//		var streamingType = IoC.Resolve<ILocalFileStreamPoolService>()
//		                       .GetLocalStream(_operationKey, _userId, FileData.Count);
//		streamingType.OpenProducerConsumerStream(streamingType.GetTempFilePath());

//		FileData.Add(new MultipartFileData(headers, streamingType.ConsumerStream.Name), streamingType.ConsumerStream);
//		return streamingType.ProducerStream;
//	}

//	public void Dispose()
//	{
//		foreach (var multipartFileData in FileData)
//		{
//			multipartFileData.Value.Dispose();
//		}
//	}
//}