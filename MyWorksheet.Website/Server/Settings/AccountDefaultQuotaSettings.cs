using ServiceLocator.Discovery.Option;

namespace MyWorksheet.Website.Server.Settings;

/// <summary>
/// Maps to the "account:quota:default" section in appsettings.json.
/// </summary>
[FromConfig("account:quota:default")]
public class AccountDefaultQuotaSettings
{
    public int Project { get; set; }
    public int Worksheet { get; set; }
    public int LocalFile { get; set; }
    public int Webhooks { get; set; }
    public int ConcurrentReports { get; set; }
}
