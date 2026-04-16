using System;
using System.IO;

namespace MyWorksheet.Website.Server.Services.ServerInfo.InfoItems.Drive;

public class DriveInfoModel
{
    public DriveInfoModel(DriveInfo info)
    {
        try
        {
            if (info.IsReady)
            {
                Format = info.DriveFormat;
                FreeSpace = info.AvailableFreeSpace;
                TotalSize = info.TotalSize;
                TotalFreeSize = info.TotalFreeSpace;
                Filled = (long)((((decimal)TotalSize) - FreeSpace) / TotalSize * 100);
                Label = info.VolumeLabel;
            }
        }
        catch (Exception)
        {
            Format = "?";
        }
        Type = info.DriveType.ToString();
        Name = info.Name;
    }

    public string Format { get; private set; }
    public string Type { get; private set; }
    public long FreeSpace { get; private set; }
    public string Name { get; private set; }
    public string Label { get; private set; }
    public long TotalSize { get; private set; }
    public long TotalFreeSize { get; private set; }

    public long Filled { get; private set; }
}