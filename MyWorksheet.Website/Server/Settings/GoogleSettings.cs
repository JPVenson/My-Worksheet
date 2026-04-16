using ServiceLocator.Discovery.Option;

namespace MyWorksheet.Website.Server.Settings;

[FromConfig("google")]
public class GoogleSettings
{
    public GoogleRecaptchaSettings Recaptcha { get; set; } = new();
}
