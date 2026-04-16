using System;
namespace MyWorksheet.Website.Server.Models;

public partial class MailBlacklist
{
    public Guid MailBlacklistId { get; set; }

    public string ClearName { get; set; }

    public string X2hash { get; set; }
}
