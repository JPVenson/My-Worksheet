using System.Linq;
using System.Text;

namespace MyWorksheet.Website.Server.Util.Auth;

public static class HexaDezimalString
{
    public static string ToDecHex(this byte[] source)
    {
        return source.ToDecHexs().Aggregate((e, f) => e + f);
    }

    public static byte[] ToHexDecByHexDec(this byte[] source)
    {
        return source.ToDecHexs().SelectMany(f => Encoding.ASCII.GetBytes(f)).ToArray();
    }

    public static string[] ToDecHexs(this byte[] source)
    {
        return source.Select(f => f.ToString("X2")).ToArray();
    }
}