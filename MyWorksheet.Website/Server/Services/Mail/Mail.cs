using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Services.Mail;

public abstract class Mail
{
    protected Mail()
    {
        Attachments = [];
        Subject = "";
    }

    public virtual string Subject { get; set; }
    public virtual string Body { get; set; }

    public List<MailAttachment> Attachments { get; set; }

    public abstract string RenderBody();
}