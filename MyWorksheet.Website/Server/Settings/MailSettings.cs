using ServiceLocator.Discovery.Option;

namespace MyWorksheet.Website.Server.Settings;

[FromConfig("mail")]
public class MailSettings
{
    public MailReceiveSettings Recive { get; set; } = new();
    public MailSendSettings Send { get; set; } = new();
}
