namespace MyWorksheet.Website.Server.Shared.Services.Logger;

public enum TrackOrTraceCategory
{
    /// <summary>
    ///     This is an Server Trace
    /// </summary>
    Trace = LoggerCategories.Server | LoggerCategories.FileUpload,

    /// <summary>
    ///     This is an Event
    /// </summary>
    Track = LoggerCategories.ServerTask | LoggerCategories.ServerBackup | LoggerCategories.ServerDbBackup | LoggerCategories.Login |
            LoggerCategories.Settings | LoggerCategories.Registration | LoggerCategories.Database | LoggerCategories.Feature |
            LoggerCategories.Project | LoggerCategories.Migration | LoggerCategories.Worksheet | LoggerCategories.Google |
            LoggerCategories.Reporting | LoggerCategories.Throttle | LoggerCategories.IoCDevice | LoggerCategories.Storage
}
