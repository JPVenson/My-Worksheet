namespace MyWorksheet.Website.Server.Services.Mail.MailTemplates;

public class TestMail : Mail
{
    public override string RenderBody()
    {
        return "Test Mail from My-Worksheet. Do NOT REPLY to this email!";
    }
}