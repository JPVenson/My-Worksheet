namespace MyWorksheet.Website.Client.Util;

public static class ByteFormatHelper
{
    public static readonly string[] NormalSuffixes = { "B", "KB", "MB", "GB", "TB" };

    public static string FormatBytes(double bytes)
    {
        var dblSByte = bytes;
        var i = 0;
        for (; i < NormalSuffixes.Length && bytes >= 1024; i++, bytes /= 1024)
        {
            dblSByte = bytes / 1024.0D;
        }

        return dblSByte.ToString("F3") + NormalSuffixes[i];
    }
}