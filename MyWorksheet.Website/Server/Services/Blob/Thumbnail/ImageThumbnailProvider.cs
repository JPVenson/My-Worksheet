using System;
using System.IO;
using MimeKit;
using SkiaSharp;

namespace MyWorksheet.Website.Server.Services.Blob.Thumbnail;

[Serializable]
public class ImageThumbnailProvider : IInMemoryThumbnailProvider
{
    public ImageThumbnailProvider()
    {
    }

    public string[] GetSupportedMimeTypes()
    {
        return new[]
        {
            MimeTypes.GetMimeType(".jpeg"),
            MimeTypes.GetMimeType(".jpg"),
            MimeTypes.GetMimeType(".bmp"),
            MimeTypes.GetMimeType(".gif"),
            MimeTypes.GetMimeType(".exif"),
            MimeTypes.GetMimeType(".png"),
            MimeTypes.GetMimeType(".tiff"),
        };
    }

    public byte[] CreateThumbnail(Stream memory, string extenstion, int x, int y)
    {
        using (var ms = new MemoryStream())
        using (var sourceStream = new MemoryStream())
        {
            memory.CopyTo(sourceStream);
            sourceStream.Seek(0, SeekOrigin.Begin);
            using (var bitmap = SKBitmap.Decode(sourceStream))
            {
                if (bitmap == null)
                {
                    return null;
                }

                int width;
                int height;
                if (bitmap.Width >= bitmap.Height)
                {
                    width = x;
                    height = (int)(bitmap.Height * x / (double)bitmap.Width);
                }
                else
                {
                    width = (int)(bitmap.Width * y / (double)bitmap.Height);
                    height = y;
                }

                using (var scaledBitmap = new SKBitmap(width, height, bitmap.ColorType, bitmap.AlphaType))
                {
                    bitmap.ScalePixels(scaledBitmap, SKSamplingOptions.Default);
                    using (var image = SKImage.FromBitmap(scaledBitmap))
                    using (var data = image.Encode(SKEncodedImageFormat.Jpeg, 80))
                    {
                        data.SaveTo(ms);
                        return ms.ToArray();
                    }
                }
            }
        }
    }

    //[SupportedOSPlatformGuard("Windows")]
    //public byte[] GenerateWindows(Stream memory, string extenstion, int x, int y)
    //{
    //	using (var bmp = Image.FromStream(memory, true, true))
    //	{
    //		using (var thumbnail = bmp.GetThumbnailImage(x, y, null, IntPtr.Zero))
    //		{
    //			using (var ms = new MemoryStream())
    //			{
    //				thumbnail.Save(ms, ImageFormat.Jpeg);
    //				return ms.ToArray();
    //			}
    //		}
    //	}
    //}

    //[SupportedOSPlatformGuard("Linux")]
    //public byte[] GenerateLinux(Stream memory, string extenstion, int x, int y)
    //{
    //	using (var bmp = Image.FromStream(memory, true, true))
    //	{
    //		using (var thumbnail = bmp.GetThumbnailImage(x, y, null, IntPtr.Zero))
    //		{
    //			using (var ms = new MemoryStream())
    //			{
    //				thumbnail.Save(ms, ImageFormat.Jpeg);
    //				return ms.ToArray();
    //			}
    //		}
    //	}
    //}
}