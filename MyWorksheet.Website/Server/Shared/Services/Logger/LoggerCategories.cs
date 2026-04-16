using System;

namespace MyWorksheet.Website.Server.Shared.Services.Logger;

[Flags]
public enum LoggerCategories
{
    Server = 1 << 0,
    ServerBackup = 1 << 1,
    ServerDbBackup = 1 << 2,
    ServerTask = 1 << 3,
    Settings = 1 << 4,
    Registration = 1 << 5,
    Database = 1 << 6,
    Feature = 1 << 7,
    Project = 1 << 8,
    Login = 1 << 9,
    Google = 1 << 10,
    Reporting = 1 << 11,
    Throttle = 1 << 12,
    FileUpload = 1 << 13,
    Worksheet = 1 << 14,
    Migration = 1 << 15,
    IoCDevice = 1 << 16,
    Storage = 1 << 17,
    Scheduler = 1 << 18
}
