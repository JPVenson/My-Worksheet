using System.IO;
using Docnet.Core;
using Docnet.Core.Models;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Skia;
using MimeKit;

namespace MyWorksheet.Website.Server.Services.Blob.Thumbnail;

public class PdfThumbnailProvider : IInMemoryThumbnailProvider
{
    public string[] GetSupportedMimeTypes()
    {
        return new[]
        {
            MimeTypes.GetMimeType(".pdf")
        };
    }

    public byte[] CreateThumbnail(Stream memory, string extenstion, int x, int y)
    {
        try
        {
            var ms = new MemoryStream();
            try
            {
                memory.CopyTo(ms);
                ms.Position = 0;

                var pageWidth = 0;
                var pageHeight = 0;
                byte[] pageImage;
                using (var docLib = DocLib.Instance)
                {
                    using (var doc = docLib.GetDocReader(ms.ToArray(), new PageDimensions(1)))
                    {
                        using (var pageReader = doc.GetPageReader(0))
                        {
                            pageImage = pageReader.GetImage(RenderFlags.RenderForPrinting);
                            pageWidth = pageReader.GetPageWidth();
                            pageHeight = pageReader.GetPageHeight();
                        }
                    }
                }

                return SkiaImage.FromStream(new MemoryStream(pageImage))
                    .Downsize(x, y, true)
                    .AsBytes();

                ////fullSizeStream.Position = 0;
                //Bitmap jpegBmp = null;
                //try
                //{
                //	jpegBmp = new Bitmap(pageWidth, pageHeight);

                //	var bitmapData = jpegBmp.LockBits(new Rectangle(0, 0, jpegBmp.Width, jpegBmp.Height),
                //									  ImageLockMode.WriteOnly,
                //									  jpegBmp.PixelFormat);
                //	Marshal.Copy(pageImage, 0, bitmapData.Scan0, pageImage.Length);
                //	jpegBmp.UnlockBits(bitmapData);

                //	using (var thumbnailImage = jpegBmp.GetThumbnailImage(x, y, null, IntPtr.Zero))
                //	{
                //		jpegBmp.Dispose();
                //		jpegBmp = null;
                //		using (var jpegStream = new MemoryStream())
                //		{
                //			thumbnailImage.Save(jpegStream, ImageFormat.Png);
                //			return jpegStream.ToArray();
                //		}
                //	}
                //}
                //finally
                //{
                //	jpegBmp?.Dispose();
                //}
            }
            finally
            {
                ms?.Dispose();
            }
        }
        finally
        {
            memory?.Dispose();
        }
    }
}

//using org.pdfclown.tools;
//using File = org.pdfclown.files.File;
//public class PdfThumbnailProvider : IInMemoryThumbnailProvider
//{
//	public bool TestExtention(string extenstion)
//	{
//		return extenstion.Equals(".pdf");
//	}

//	public byte[] CreateThumbnail(Stream memory, string extenstion, int x, int y)
//	{
//		using (memory)
//		{
//			using (var file = new File(new org.pdfclown.bytes.Stream(memory)))
//			{
//				var rasterrizer = new Renderer();
//				var firstPage = file.Document.Pages.First();
//				using (var renderResult = rasterrizer.Render(firstPage, firstPage.Size))
//				{
//					using (var thumbnailResult = renderResult.GetThumbnailImage(x, y, null, IntPtr.Zero))
//					{
//						using (var resultCache = new MemoryStream())
//						{
//							thumbnailResult.Save(resultCache, ImageFormat.Jpeg);
//							return resultCache.ToArray();
//						}
//					}
//				}
//			}
//		}
//	}
//}