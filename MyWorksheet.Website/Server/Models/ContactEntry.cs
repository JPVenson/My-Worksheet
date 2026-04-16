using System;
namespace MyWorksheet.Website.Server.Models;

public partial class ContactEntry
{
    public Guid ContactEntryId { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    public string Message { get; set; }

    public string SenderIp { get; set; }

    public string ContactType { get; set; }
}
