using System;
using System.IO;
using MyWorksheet.Website.Server.Settings;
using Microsoft.Extensions.Options;

namespace MyWorksheet.Website.Server.Services.StreamPool;

public class StreamingType
{
    private readonly ILocalFileStreamPoolService _localFileStreamPoolService;
    private readonly IOptions<AppServerSettings> _serverSettings;

    public StreamingType(string streamingKind,
        Guid userId,
        string key,
        ILocalFileStreamPoolService fileStreamPoolService,
        IOptions<AppServerSettings> serverSettings)
    {
        StreamingKind = streamingKind;
        UserId = userId;
        Key = key;
        _localFileStreamPoolService = fileStreamPoolService;
        _serverSettings = serverSettings;
    }

    public string StreamingKind { get; private set; }
    public Guid UserId { get; private set; }
    public string Key { get; private set; }

    public NotifyFileStream ConsumerStream { get; private set; }
    public NotifyFileStream ProducerStream { get; private set; }

    public virtual string GetTempFilePath(string ensureExtention = ".temp")
    {
        return Path.Combine(Environment.ExpandEnvironmentVariables(_serverSettings.Value.Storage.File.Temp ?? "/tmp/"),
            Guid.NewGuid().ToString("N") + ensureExtention);
    }

    public FileStream CreateTempStream(string ensureExtention = ".temp")
    {
        return OpenProducerStream(GetTempFilePath(ensureExtention));
    }

    public void OpenProducerConsumerStream(string path)
    {
        if (ProducerStream != null)
        {
            throw new InvalidOperationException("The Object was once created. Dispose it");
        }
        if (ConsumerStream != null)
        {
            throw new InvalidOperationException("The Object was once created. Dispose it");
        }
        ProducerStream = new NotifyFileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, 4096,
            FileOptions.SequentialScan);
        ConsumerStream = new NotifyFileStream(ProducerStream.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, FileOptions.SequentialScan);

        ProducerStream.Disposed += ProducerStream_Disposed;
        ConsumerStream.Disposed += ProducerStream_Disposed;
    }

    private void ProducerStream_Disposed(object sender, EventArgs e)
    {
        if (ConsumerStream == null || ProducerStream == null)
        {
            _localFileStreamPoolService.Unregister(Key);
            File.Delete((ProducerStream ?? ConsumerStream).Name);
        }

        if (sender == ConsumerStream)
        {
            if (ConsumerStream != null)
            {
                ConsumerStream.Disposed -= ProducerStream_Disposed;
            }

            ConsumerStream = null;
        }

        if (sender == ProducerStream)
        {
            if (ProducerStream != null)
            {
                ProducerStream.Disposed -= ProducerStream_Disposed;
            }

            ProducerStream = null;
        }
    }

    public FileStream OpenProducerStream(string path)
    {
        if (ProducerStream != null)
        {
            throw new InvalidOperationException("The Object was once created. Dispose it");
        }

        ProducerStream = new NotifyFileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, 4096,
            FileOptions.SequentialScan & FileOptions.DeleteOnClose);
        _localFileStreamPoolService.RegisterForFinalisation(Key, ProducerStream);
        return ProducerStream;
    }
}