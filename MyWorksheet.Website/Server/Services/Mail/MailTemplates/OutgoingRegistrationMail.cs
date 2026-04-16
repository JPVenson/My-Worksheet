using MyWorksheet.Website.Server.Services.FileSystem;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Address;

namespace MyWorksheet.Website.Server.Services.Mail.MailTemplates;

public class OutgoingRegistrationMail : TemplateMail
{
    private readonly string _password;
    private readonly string _mail;
    private readonly string _username;

    public OutgoingRegistrationMail(string password, string mail, string username, AddressModel modelAddress,
        ILocalFileProvider localFileProvider) : base(localFileProvider)
    {
        TemplatePath = "/StaticViews/EmailTemplates/RegistrationMail.html";
        _password = password;
        _mail = mail;
        _username = username;
        //_confirmCode = confirmCode;

        Subject = "Your Registration on My-Worksheet.com";
        Values.Add("Title", Subject);
        Values.Add("MailAddress", mail);
        Values.Add("Password", password);
        Values.Add("Username", username);
        Values.Add("Address", modelAddress);
    }
}