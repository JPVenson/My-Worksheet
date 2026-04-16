using System;
using System.IO;

namespace MyWorksheet.Website.Server.Services.StreamPool;

public class NotifyFileStream : FileStream
{
    /// <inheritdoc />
    public NotifyFileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize,
        FileOptions options) : base(path, mode, access, share, bufferSize, options)
    {
    }

    public event EventHandler Disposed;

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        OnDisposed();
    }

    protected virtual void OnDisposed()
    {
        Disposed?.Invoke(this, EventArgs.Empty);
    }
}