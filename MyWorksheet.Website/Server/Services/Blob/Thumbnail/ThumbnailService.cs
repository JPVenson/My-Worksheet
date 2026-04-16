using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.FileSystem;
using MyWorksheet.Website.Server.Services.ServerManager;
using MimeKit;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Services.Blob.Thumbnail;

[SingletonService(typeof(IThumbnailService), typeof(IRequireInit), typeof(IReportCapability))]
public class ThumbnailService : RequireInit, IThumbnailService
{
    private readonly ILocalFileProvider _localFileProvider;
    private IDictionary<string, byte[]> _defaultThumbnails;

    public IDictionary<string, Size> ThumbnailSizes { get; private set; }

    public Size DefaultSize { get; set; }

    public ThumbnailService(ILocalFileProvider localFileProvider)
    {
        _localFileProvider = localFileProvider;
        _defaultThumbnails = new Dictionary<string, byte[]>();

        ThumbnailSizes = new Dictionary<string, Size>
        {
            { "xs", DefaultSize = new Size(96, 96) },
            { "md", new Size(512, 512) },
            { "xl", new Size(1024, 1024) }
        };
        InMemoryThumbnailProviders = [new ImageThumbnailProvider(), new PdfThumbnailProvider()];
        //FallbackProvider = new Win32RegDefaultIconProvider();
    }

    public override async ValueTask InitAsync()
    {
        var imageProvider = InMemoryThumbnailProviders.FirstOrDefault();

        using (var memoryStream = await _localFileProvider.ReadAllAsync("/Content/Images/DefaultFileIcon.jpg"))
        {
            foreach (var thumbnailSize in ThumbnailSizes)
            {
                memoryStream.Seek(0, SeekOrigin.Begin);
                _defaultThumbnails.Add(thumbnailSize.Key,
                    imageProvider.CreateThumbnail(memoryStream, ".jpeg", thumbnailSize.Value.Height, thumbnailSize.Value.Width));
            }
        }
    }

    public ICollection<IInMemoryThumbnailProvider> InMemoryThumbnailProviders { get; set; }

    public IInMemoryThumbnailProvider FallbackProvider { get; set; }

    public async Task<byte[]> CreateThumbnailFromFileDetailed(Stream content, string fileName, string contentType, string size)
    {
        var sizeOfThumbnail = ThumbnailSizes[size];

        content.Seek(0, SeekOrigin.Begin);

        var tryGetIcon = new NestedTry(() =>
        {
            var hasManagedProvider = InMemoryThumbnailProviders
                .FirstOrDefault(e =>
                    e.GetSupportedMimeTypes()
                        .Any(f => f.Equals(contentType, StringComparison.OrdinalIgnoreCase)));

            if (hasManagedProvider == null)
            {
                return null;
            }

            return new DomainGate().CreateThumbnail(hasManagedProvider, content, fileName, sizeOfThumbnail.Height, sizeOfThumbnail.Width);
        }).Otherwise(() =>
        {
            return _defaultThumbnails[size];
        });

        var result = await tryGetIcon.InvokeChainAsync();
        if (result == null)
        {
            return null;
        }
        //if (result is IDisposable bitmap)
        //{
        //	using (bitmap)
        //	{
        //		if (!(bitmap is Bitmap bitmap1))
        //		{
        //			return null;
        //		}

        //		using (var ms = new MemoryStream())
        //		{
        //			bitmap1.Save(ms, ImageFormat.Jpeg);
        //			return ms.ToArray();
        //		}
        //	}
        //}
        if (result is byte[] bytes)
        {
            return bytes;
        }
        return null;
    }

    public byte[] DefaultThumbnail(string size)
    {
        return _defaultThumbnails[size];
    }

    public bool CanCreateThumbnail(string fileName)
    {
        return InMemoryThumbnailProviders.Any(e => e.GetSupportedMimeTypes()
            .Any(f => f.Equals(MimeTypes.GetMimeType(fileName), StringComparison.OrdinalIgnoreCase)));
    }

    public ProcessorCapability[] ReportCapabilities()
    {
        return this.InMemoryThumbnailProviders
            .SelectMany(e => e.GetSupportedMimeTypes())
            .Select(e => "Feature_Thumbnail_WitExtension_" + e)
            .Select(e => new ProcessorCapability()
            {
                Name = e,
                Value = "",
                IsEnabled = true,
            })
            .ToArray();
    }
}