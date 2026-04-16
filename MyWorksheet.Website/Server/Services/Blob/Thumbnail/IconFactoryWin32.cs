using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace MyWorksheet.Website.Server.Services.Blob.Thumbnail;

public class IconFactoryWin32
{
    [Flags]
    public enum SIIGBF
    {
        SIIGBF_RESIZETOFIT = 0x00000000,
        SIIGBF_BIGGERSIZEOK = 0x00000001,
        SIIGBF_MEMORYONLY = 0x00000002,
        SIIGBF_ICONONLY = 0x00000004,
        SIIGBF_THUMBNAILONLY = 0x00000008,
        SIIGBF_INCACHEONLY = 0x00000010,
        SIIGBF_CROPTOSQUARE = 0x00000020,
        SIIGBF_WIDETHUMBNAILS = 0x00000040,
        SIIGBF_ICONBACKGROUND = 0x00000080,
        SIIGBF_SCALEUP = 0x00000100,
    }

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    public static extern int SHCreateItemFromParsingName(string path, IntPtr pbc, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IShellItemImageFactory factory);

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("bcc18b79-ba16-442f-80c4-8a59c30c463b")]
    public interface IShellItemImageFactory
    {
        [PreserveSig]
        int GetImage(Size size, SIIGBF flags, out IntPtr phbm);
    }

    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    public static extern Int32 SHParseDisplayName(
        [MarshalAs(UnmanagedType.LPWStr)] String pszName,
        IntPtr pbc,
        out IntPtr ppidl,
        UInt32 sfgaoIn,
        out UInt32 psfgaoOut);

    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    internal static extern void SHChangeNotify(
        UInt32 wEventId,
        UInt32 uFlags,
        IntPtr dwItem1,
        IntPtr dwItem2);

    [Flags]
    private enum ShellChangeNotificationEvents : uint
    {
        //...
        SHCNE_UPDATEITEM = 0x00002000,
        //...
    }

    private enum ShellChangeNotificationFlags
    {
        //...
        SHCNF_FLUSH = 0x1000,
        //...
    }

    public static void refreshThumbnail(string path)
    {
        try
        {
            uint iAttribute;
            IntPtr pidl;
            SHParseDisplayName(path, IntPtr.Zero, out pidl, 0, out iAttribute);
            SHChangeNotify(
                (uint)ShellChangeNotificationEvents.SHCNE_UPDATEITEM,
                (uint)ShellChangeNotificationFlags.SHCNF_FLUSH,
                pidl,
                IntPtr.Zero);
        }
        catch { }
    }
}