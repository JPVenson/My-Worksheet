using System;
using MyWorksheet.Website.Server.Services.FileSystem;
using MyWorksheet.Website.Server.Settings;

namespace MyWorksheet.Website.Server.Services.Mail.MailTemplates;

public class OutgingResetPasswordMail : TemplateMail
{
    public OutgingResetPasswordMail(string username, string operatingSystem, string browserName,
        string passwordResetToken, DateTime requestDate, ILocalFileProvider localFileProvider,
        AppServerSettings serverSettings)
        : base(localFileProvider)
    {
        TemplatePath = "/StaticViews/EmailTemplates/PasswordReset.html";
        Values.Add("name", username);
        Values.Add("operating_system", operatingSystem);
        Values.Add("browser_name", browserName);
        Values.Add("date", requestDate);
        Subject = "Password Request";

        var primaryRealm = serverSettings.Realm.PrimaryRealm;
        var primaryRealmPort = serverSettings.Realm.Port;
        var uriBuilder = new UriBuilder("https", primaryRealm, primaryRealmPort, "/Account/Password-Reset");
        uriBuilder.Query = "code=" + Uri.EscapeDataString(passwordResetToken ?? "");
        Values.Add("action_url", uriBuilder.ToString());
    }
}