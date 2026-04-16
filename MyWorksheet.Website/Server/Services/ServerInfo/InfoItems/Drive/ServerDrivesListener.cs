using System;
using System.IO;
using System.Linq;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;

namespace MyWorksheet.Website.Server.Services.ServerInfo.InfoItems.Drive;

public class ServerDrivesListener : ServerPullInfoListener
{
    public ServerDrivesListener(Action<string, object> publishValue, IAppLogger logger) : base(publishValue, logger, 10500)
    {
        Consumers.Add(PublishValue);
    }

    protected override void OnDispose()
    {
        Consumers.Remove(PublishValue);
        base.OnDispose();
    }

    public override string Key { get; } = "Drives";
    public override void PublishValue()
    {
        var allDrives = DriveInfo.GetDrives().Select(e => new DriveInfoModel(e)).ToArray();
        Publish(allDrives);
    }
}