using System;
using MyWorksheet.Website.Server.Services.FileSystem;

namespace MyWorksheet.Website.Server.Services.Mail.MailTemplates;

public class FailedTaskMail : TemplateMail
{
    public FailedTaskMail(string nameOfTask, DateTime failedAt, Exception reason,
        ILocalFileProvider localFileProvider)
        : base(localFileProvider)
    {
        TemplatePath = "/StaticViews/EmailTemplates/ContactRequest.html";
        Values.Add("name", nameOfTask);
        Values.Add("failedAt", failedAt);
        Values.Add("reason", reason.ToString());
        Subject = "Task Failed";
    }
}