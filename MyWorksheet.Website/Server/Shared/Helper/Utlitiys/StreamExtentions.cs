using System.IO;
using System.Text;

namespace MyWorksheet.Webpage.Helper.Utlitiys;

public static class StreamExtentions
{
    public static string Stringify(this Stream source, bool disposeOriginal, Encoding encoding)
    {
        try
        {
            source.Seek(0, SeekOrigin.Begin);
            var stream = source as MemoryStream;
            if (stream != null)
            {
                return encoding.GetString(stream.ToArray());
            }

            using (var ms = new MemoryStream())
            {
                source.CopyToAsync(ms);
                return ms.Stringify(disposeOriginal, encoding);
            }
        }
        finally
        {
            if (disposeOriginal)
            {
                source.Dispose();
            }
        }
    }
}