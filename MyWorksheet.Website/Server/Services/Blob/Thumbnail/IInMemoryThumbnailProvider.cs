using System.IO;

namespace MyWorksheet.Website.Server.Services.Blob.Thumbnail;

public interface IInMemoryThumbnailProvider
{
    string[] GetSupportedMimeTypes();
    byte[] CreateThumbnail(Stream memory, string extenstion, int x, int y);
}