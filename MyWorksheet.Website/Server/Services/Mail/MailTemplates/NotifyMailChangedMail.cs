using MyWorksheet.Website.Server.Services.FileSystem;

namespace MyWorksheet.Website.Server.Services.Mail.MailTemplates;

public class NotifyMailChangedMail : TemplateMail
{
    public NotifyMailChangedMail(string newMail, ILocalFileProvider localFileProvider)
        : base(localFileProvider)
    {
        Values.Add("newMailAddress", newMail);
        TemplatePath = "/StaticViews/EmailTemplates/NotifyMailHasChangedMail.html";
        Subject = "Mail Changed";
    }
}