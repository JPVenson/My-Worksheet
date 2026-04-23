using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MyWorksheet.Website.Server.Util.Auth;

namespace MyWorksheet.Website.Shared.Util;

public static class ChallangeUtil
{
    public static string Pepper { get; set; } = "02EB94B6-0ECF-4744-A882-E9A2047E3E30";

    public static string ShiftString(string source, byte[] by)
    {
        var chList = source.Select(f => f).ToArray();

        var sb = new StringBuilder();
        for (var i = 0; i < chList.Length; i++)
        {
            var shiftTo = chList[i];
            var f = i;
            if (i >= @by.Length)
            {
                f = i - @by.Length;
            }
            var shifted = shiftTo << @by[f]; //TODO hacky
            sb.Append(MakeReadable(shifted));
        }
        return sb.ToString();
    }

    public static string MakeReadable(long source)
    {
        if (source < 0)
        {
            source = source * -1;
        }

        if (source <= 255)
        {
            return source.ToString("X2");
        }

        return MakeReadable(source / 255); // restrict to ascii
    }

    public static byte[] HashPassword(string clear, string username)
    {
        using (var sha256 = SHA256.Create())
        {
            var pw = clear + username + Pepper;
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(pw));
        }
    }

    public static byte[] HashPasswordShort(string clear)
    {
        using (var sha256 = new MD5CryptoServiceProvider())
        {
            var pw = clear + Pepper;
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(pw));
        }
    }

    public static byte[] StringToByteArrayFastest(string hex)
    {
        if (hex.Length % 2 == 1)
            throw new Exception("The binary key cannot have an odd number of digits");

        byte[] arr = new byte[hex.Length >> 1];

        for (int i = 0; i < hex.Length >> 1; ++i)
        {
            arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
        }

        return arr;
    }

    public static int GetHexVal(char hex)
    {
        int val = (int)hex;
        //For uppercase A-F letters:
        return val - (val < 58 ? 48 : 55);
        //For lowercase a-f letters:
        //return val - (val < 58 ? 48 : 87);
        //Or the two combined, but a bit slower:
        //return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
    }

    public static string EncryptPassword(string clear, string username)
    {
        using (var des = new RijndaelManaged())
        {
            //des.Padding = PaddingMode.None;
            using (var pdb = new Rfc2898DeriveBytes(Pepper, HashPasswordShort(username)))
            {
                des.Key = pdb.GetBytes(32);
                des.IV = pdb.GetBytes(16);
            }

            var sourceData = Encoding.UTF8.GetBytes(clear);
            using (var targetStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(targetStream, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(sourceData, 0, sourceData.Length);
                    cryptoStream.FlushFinalBlock();
                    return targetStream.ToArray().ToDecHex();
                }
            }
        }
    }

    public static string DecryptPassword(string data, string username)
    {
        using (var des = new RijndaelManaged())
        {
            //des.Padding = PaddingMode.None;
            using (var pdb = new Rfc2898DeriveBytes(Pepper, HashPasswordShort(username)))
            {
                des.Key = pdb.GetBytes(32);
                des.IV = pdb.GetBytes(16);
            }

            var sourceData = StringToByteArrayFastest(data);
            using (var targetStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(targetStream, des.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(sourceData, 0, sourceData.Length);
                    cryptoStream.FlushFinalBlock();
                    return Encoding.UTF8.GetString(targetStream.ToArray());
                }
            }
        }
    }
}
