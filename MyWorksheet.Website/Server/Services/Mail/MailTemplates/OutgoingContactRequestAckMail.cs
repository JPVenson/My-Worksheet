using MyWorksheet.Website.Server.Services.FileSystem;

namespace MyWorksheet.Website.Server.Services.Mail.MailTemplates;

public class OutgoingContactRequestAckMail : TemplateMail
{
    public OutgoingContactRequestAckMail(string nameOfCaller, string emailAddressOfCaller, string message,
        string entityContactType, ILocalFileProvider localFileProvider)
        : base(localFileProvider)
    {
        TemplatePath = "/StaticViews/EmailTemplates/ContactRequestAck.html";
        Values.Add("name", nameOfCaller);
        Values.Add("mail", emailAddressOfCaller);
        Values.Add("message", message);
        Values.Add("type", entityContactType);
        Subject = "Contact Request";
    }
}