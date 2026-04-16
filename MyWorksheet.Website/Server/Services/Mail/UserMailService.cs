using System.Threading.Tasks;
using System;
using Katana.CommonTasks.Models;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Mail.MailTemplates;

namespace MyWorksheet.Website.Server.Services.Mail;

public class UserMailService : ApplicationMailService, IUserMailService
{
    private readonly MailAccount _mailAccount;

    public UserMailService(MailAccount mailAccount, string password)
        : base(mailAccount.EmailAddress, password, mailAccount.Username, mailAccount.ServerAddress, mailAccount.ServerPort, mailAccount.Protocol)
    {
        _mailAccount = mailAccount;
    }

    //public override void PreSendMail(Mail mail, MailMessage mailMessage, params string[] recipients)
    //{
    //	var blobManagerService = IoC.Resolve<IBlobManagerService>();

    //	var mailBody = blobManagerService.SetData(
    //		new BlobData(new MemoryStream(Encoding.Default.GetBytes(mail.Body)), "Mail-Body.bin"),
    //		_mailAccount.StorageProviderId, _mailAccount.IdAppUser, StorageEntityType.Report);



    //	return await base.SendMail(mail, recipients);
    //}


    public async Task<QuestionableBoolean> Test(Guid userId)
    {
        var recipent = MailSettings?.Value.Send.Sender ?? Mail;
        return await SendMail(new TestMail(), userId, recipent);
    }
}