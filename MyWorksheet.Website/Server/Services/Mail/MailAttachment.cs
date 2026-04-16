using System;
using System.IO;
using System.Threading.Tasks;

namespace MyWorksheet.Website.Server.Services.Mail;

public abstract class MailAttachment : IDisposable
{
    public string Name { get; set; }
    public string FileType { get; set; }
    public MemoryStream Content { get; set; }
    public abstract Task Init();

    public void Dispose()
    {
        Content?.Dispose();
    }
}