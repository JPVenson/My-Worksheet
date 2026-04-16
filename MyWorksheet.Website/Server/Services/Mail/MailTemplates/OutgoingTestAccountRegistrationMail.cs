using MyWorksheet.Website.Server.Services.FileSystem;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Address;

namespace MyWorksheet.Website.Server.Services.Mail.MailTemplates;

public class OutgoingTestAccountRegistrationMail : OutgoingRegistrationMail
{
    public OutgoingTestAccountRegistrationMail(string password, string mail, string username,
        AddressModel modelAddress, ILocalFileProvider localFileProvider)
        : base(password, mail, username, modelAddress, localFileProvider)
    {
        base.TemplatePath = "/StaticViews/EmailTemplates/TestRegistrationMail.html";
        Subject = "Test Account Request";
    }
}