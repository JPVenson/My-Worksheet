using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Services.ServerManager;

namespace MyWorksheet.Website.Server.Services.Blob.Thumbnail;

public interface IThumbnailService : IReportCapability
{
    Task<byte[]> CreateThumbnailFromFileDetailed(Stream content, string fileName, string contentType, string size);
    byte[] DefaultThumbnail(string size);
    //string DefaultThumbnailBase64();
    bool CanCreateThumbnail(string filename);

    IDictionary<string, Size> ThumbnailSizes { get; }
}