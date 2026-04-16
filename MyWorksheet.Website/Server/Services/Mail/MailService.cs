using System;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Settings;
using MyWorksheet.Website.Shared.Util;
using Microsoft.Extensions.Options;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Services.Mail;

[SingletonService(typeof(IMailServiceProvider))]
public class MailService : IMailServiceProvider
{
    public MailService(IOptions<MailSettings> mailSettings, IOptions<TransformationSettings> transformationSettings)
    {
        ApplicationMailService = new ApplicationMailService(mailSettings, transformationSettings);
    }

    public IMailService ApplicationMailService { get; }

    public IUserMailService UserMailService(MyworksheetContext db, Guid userMailId)
    {
        var mailAccount = db.MailAccounts.Find(userMailId);
        if (mailAccount == null)
        {
            return null;
        }

        var password = mailAccount.Password;

        if (password != null)
        {
            var appUser = db.AppUsers.Find(mailAccount.IdAppUser);
            password = ChallangeUtil.DecryptPassword(password, appUser.Username);
        }

        return new UserMailService(mailAccount, password);
    }
}