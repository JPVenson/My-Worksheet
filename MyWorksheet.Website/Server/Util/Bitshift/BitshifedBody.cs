using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace MyWorksheet.Website.Server.Util.Bitshift;

public class BitshifedBody : BitshiftedStream, IHttpResponseBodyFeature
{
    private readonly HttpContext _context;
    private readonly IHttpResponseBodyFeature _originalBodyFeature;
    private PipeWriter? _pipeAdapter;
    private bool _complete;

    public BitshifedBody(
        HttpContext context,
        IHttpResponseBodyFeature originalBodyFeature, byte[] key, byte[] iv)
        : base(originalBodyFeature.Stream, key, iv)
    {
        _context = context;
        _originalBodyFeature = originalBodyFeature;
    }

    /// <inheritdoc />
    public void DisableBuffering()
    {
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        return _originalBodyFeature.StartAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task SendFileAsync(string path, long offset, long? count, CancellationToken cancellationToken = new CancellationToken())
    {
        //_context.Response.Headers.ContentLength = (_context.Response.Headers.ContentLength / 16 + 1) * 16;
        // Adds the compression headers for HEAD requests even if the body was not used.
        if (HttpMethods.IsHead(_context.Request.Method))
        {
            return;
        }
        await SendFileFallback.SendFileAsync(Stream, path, offset, count, cancellationToken);
        //await Stream.FlushAsync(cancellationToken);
        //await Stream.DisposeAsync();
    }

    /// <inheritdoc />
    public async Task CompleteAsync()
    {
        await FinishCompressionAsync(); // Sets _complete
        await _originalBodyFeature.CompleteAsync();
    }

    internal async Task FinishCompressionAsync()
    {
        if (_complete)
        {
            return;
        }

        _complete = true;

        if (_pipeAdapter != null)
        {
            await _pipeAdapter.CompleteAsync();
        }

        await DisposeAsync();

        //// Adds the compression headers for HEAD requests even if the body was not used.
        //if (HttpMethods.IsHead(_context.Request.Method))
        //{
        //	_context.Response.Headers.ContentLength = (_context.Response.Headers.ContentLength / 16 + 1) * 16;
        //}
    }

    /// <inheritdoc />
    public Stream Stream => this;


    public PipeWriter Writer
    {
        get
        {
            if (_pipeAdapter == null)
            {
                _pipeAdapter = PipeWriter.Create(Stream, new StreamPipeWriterOptions(leaveOpen: false));
            }

            return _pipeAdapter;
        }
    }
}
