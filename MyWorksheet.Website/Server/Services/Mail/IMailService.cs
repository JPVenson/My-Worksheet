using System.Threading.Tasks;
using System;
using Katana.CommonTasks.Models;

namespace MyWorksheet.Website.Server.Services.Mail;

public interface IMailService
{
    string Mail { get; set; }
    string UserName { get; set; }
    string Server { get; set; }
    string MailType { get; set; }
    int Port { get; set; }
    string Password { get; set; }

    bool IsTestRealm { get; set; }

    void PreProcessMail(Mail mail);
    Task<QuestionableBoolean> SendMail(Mail mail, Guid userId, params string[] recipients);
}