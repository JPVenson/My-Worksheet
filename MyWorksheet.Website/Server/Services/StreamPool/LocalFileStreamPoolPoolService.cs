using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using MyWorksheet.Website.Server.Shared.Services.Logger;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using MyWorksheet.Website.Shared.Services.Activation;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Services.StreamPool;

[SingletonService(typeof(ILocalFileStreamPoolService))]
public class LocalFileStreamPoolPoolService : ILocalFileStreamPoolService
{
    private readonly IAppLogger _logger;
    private readonly ActivatorService _activatorService;

    public LocalFileStreamPoolPoolService(IAppLogger logger, ActivatorService activatorService)
    {
        _logger = logger;
        _activatorService = activatorService;
        StreamsPerOperation = new ConcurrentDictionary<string, StreamingType>();
    }

    public const string OPKEY_ADD_WORKFLOW_FILE = nameof(OPKEY_ADD_WORKFLOW_FILE);
    public static string OPKEY_GENERATE_TEMPLATE = nameof(OPKEY_GENERATE_TEMPLATE);
    public const string OPKEY_ADD_WORKSHEET_ASSERT = nameof(OPKEY_ADD_WORKSHEET_ASSERT);
    public const string OPKEY_STORAGE_PROVIDER = nameof(OPKEY_STORAGE_PROVIDER);
    public const string OPKEY_EXTRACT_THUMBNAIL = nameof(OPKEY_EXTRACT_THUMBNAIL);

    public IAppLogger Logger { get; set; }

    public ConcurrentDictionary<string, StreamingType> StreamsPerOperation { get; private set; }

    public void RegisterForFinalisation(string key, NotifyFileStream fileStream)
    {
        fileStream.Disposed += (o, eventArgs) =>
        {
            if (StreamsPerOperation.TryRemove(key, out var existing))
            {
            }
        };
    }

    public void Unregister(string key)
    {
        StreamsPerOperation.TryRemove(key, out var existing);
    }

    public StreamingType GetLocalStream(string operation, Guid userId, int operationCounter)
    {
        var type = StreamsPerOperation.GetOrAdd(operation + "|" + operationCounter + "|" + userId, f =>
        {
            StreamingType existing;
            if (!StreamsPerOperation.TryGetValue(f, out existing))
            {
                return _activatorService.ActivateType<StreamingType>(operation, userId, f, this);
            }

            existing.ConsumerStream?.Dispose();
            existing.ProducerStream?.Dispose();
            _logger.LogWarning("Detected undisposed Stream to file", LoggerCategories.Server.ToString(),
                new Dictionary<string, string>
                {
                    {"Key", f},
                    {"File", existing.ProducerStream?.Name}
                });

            return _activatorService.ActivateType<StreamingType>(operation, userId, f);
        });
        return type;
    }
}