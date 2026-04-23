using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MyWorksheet.Website.Server.Util.Bitshift;

public class BitshiftedStream : Stream
{
    private readonly Stream _sourceStream;
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public BitshiftedStream(Stream sourceStream, byte[] key, byte[] iv)
    {
        _sourceStream = sourceStream;
        _key = key;
        _iv = iv;
    }

    /// <inheritdoc />
    public override Task FlushAsync(CancellationToken cancellationToken)
    {
        return _sourceStream.FlushAsync(cancellationToken);
    }

    /// <inheritdoc />
    public override void Flush()
    {
        _sourceStream.Flush();
    }

    /// <inheritdoc />
    public override int Read(byte[] buffer, int offset, int count)
    {
        return _sourceStream.Read(buffer, offset, count);
    }

    /// <inheritdoc />
    public override ValueTask DisposeAsync()
    {
        return _sourceStream.DisposeAsync();
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        _sourceStream.Dispose();
    }

    /// <inheritdoc />
    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc />
    public override void SetLength(long value)
    {
        throw new System.NotImplementedException();
    }

    private int _counter = 1;

    /// <inheritdoc />
    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        for (int i = offset; i < count; i++)
        {
            buffer[i] = (byte)(buffer[i] ^ _key[_counter++ % _key.Length]);
        }
        return _sourceStream.WriteAsync(buffer, offset, count, cancellationToken);
    }

    /// <inheritdoc />
    public override void Write(byte[] buffer, int offset, int count)
    {
        for (int i = offset; i < count; i++)
        {
            buffer[i] = (byte)(buffer[i] ^ _key[_counter++ % _key.Length]);
        }

        _sourceStream.Write(buffer, offset, count);
    }

    /// <inheritdoc />
    public override bool CanRead
    {
        get
        {
            return _sourceStream.CanRead;
        }
    }

    /// <inheritdoc />
    public override bool CanSeek { get; } = false;

    /// <inheritdoc />
    public override bool CanWrite
    {
        get
        {
            return _sourceStream.CanWrite;
        }
    }

    /// <inheritdoc />
    public override long Length
    {
        get
        {
            return _sourceStream.Length;
        }
    }

    /// <inheritdoc />
    public override long Position
    {
        get { return _sourceStream.Length; }
        set
        {
            throw new System.NotImplementedException();
        }
    }
}
