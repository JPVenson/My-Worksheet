using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Services.FileSystem;

/// <summary>
///		Service for Inversion Of Control usage
/// </summary>
public interface ILocalFileProvider
{
    /// <summary>
    ///		Maps a Url resource path part
    /// </summary>
    string Map(string source);

    /// <summary>
    ///		Maps the Url Resource path part and then executes the chain of Middelware
    /// </summary>
    Task<Stream> ReadAllAsync(string url);

    /// <summary>
    ///		Maps the Url Resource and then writes to the target location
    /// </summary>
    /// <param name="url"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    Task WriteAll(string url, byte[] content);

    string PhysicalRootPath { get; set; }

    /// <summary>
    ///		Maps the Url and then checks for content available 
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    bool Exists(string url);
}

[SingletonService(typeof(ILocalFileProvider))]
public class LocalFileProvider : ILocalFileProvider
{
    private readonly PhysicalFileProvider _fileProvider;

    public LocalFileProvider(IHostEnvironment hostEnvironment)
    {
        _fileProvider = new PhysicalFileProvider(hostEnvironment.ContentRootPath);
    }

    public string Map(string source)
    {
        return null;
    }

    public async Task<Stream> ReadAllAsync(string url)
    {
        return _fileProvider.GetFileInfo(url).CreateReadStream();
    }

    public async Task WriteAll(string url, byte[] content)
    {

    }

    public string PhysicalRootPath { get; set; }
    public bool Exists(string url)
    {
        return _fileProvider.GetFileInfo(url).Exists;
    }
}