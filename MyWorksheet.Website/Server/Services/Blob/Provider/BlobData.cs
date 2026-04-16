using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using MimeKit;

namespace MyWorksheet.Website.Server.Services.Blob.Provider;

public class BlobData : IDisposable
{
    public BlobData(Stream stream, string fileName)
    {
        DataStream = stream;
        Filename = fileName;
        foreach (var invalidFileNameChar in Path.GetInvalidFileNameChars())
        {
            Filename = Filename.Replace(invalidFileNameChar.ToString(), "");
        }

        MimeType = MimeTypes.GetMimeType(Path.GetExtension(Filename));
    }

    protected BlobData()
    {

    }

    public static BlobData GetTestData(MemoryStream stream)
    {
        return new BlobData()
        {
            DataStream = stream,
            Filename = Guid.NewGuid().ToString() + ".txt",
        };
    }

    public virtual Stream DataStream { get; set; }
    public virtual string Filename { get; set; }
    public virtual string MimeType { get; set; }

    public void Dispose()
    {
        DataStream?.Dispose();
    }
}

public class BlobDataCore : BlobData
{
    private readonly IFormFile _formFile;
    private Stream _stream;

    public BlobDataCore(IFormFile formFile)
    {
        _formFile = formFile;
        Filename = Path.GetFileName(formFile.FileName);
        foreach (var invalidFileNameChar in Path.GetInvalidFileNameChars())
        {
            Filename = Filename.Replace(invalidFileNameChar.ToString(), "");
        }
        MimeType = MimeTypes.GetMimeType(Path.GetExtension(Filename));
    }

    public override Stream DataStream
    {
        get
        {
            return _stream ?? (_stream = _formFile.OpenReadStream());
        }
    }
}