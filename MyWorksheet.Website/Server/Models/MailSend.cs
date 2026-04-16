using System;

namespace MyWorksheet.Website.Server.Models;

public partial class MailSend
{
    public Guid MailSendId { get; set; }

    public Guid IdMailAccount { get; set; }

    public Guid IdContent { get; set; }

    public Guid? IdAttachment { get; set; }

    public DateTime? SendAt { get; set; }

    public string Recipients { get; set; }

    public bool Success { get; set; }

    public int ResendCount { get; set; }

    public virtual StorageEntry IdAttachmentNavigation { get; set; }

    public virtual StorageEntry IdContentNavigation { get; set; }

    public virtual MailAccount IdMailAccountNavigation { get; set; }
}
