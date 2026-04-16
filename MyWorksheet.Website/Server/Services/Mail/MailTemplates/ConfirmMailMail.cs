using MyWorksheet.Website.Server.Services.FileSystem;

namespace MyWorksheet.Website.Server.Services.Mail.MailTemplates;

public class ConfirmMailMail : TemplateMail
{
    public ConfirmMailMail(string mailCode, ILocalFileProvider localFileProvider)
        : base(localFileProvider)
    {
        Values.Add("confirmAddress", mailCode);
        TemplatePath = "/StaticViews/EmailTemplates/ContactRequest.html";
        Subject = "Confirm E-Mail address";
    }
}