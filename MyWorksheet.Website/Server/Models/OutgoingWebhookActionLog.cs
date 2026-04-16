using System;

namespace MyWorksheet.Website.Server.Models;

public partial class OutgoingWebhookActionLog : IUserRelation
{
    public Guid OutgoingWebhookActionLogId { get; set; }

    public bool Success { get; set; }

    public string Error { get; set; }

    public int ReturnCode { get; set; }

    public DateTime DateOfAction { get; set; }

    public string InitiatorIp { get; set; }

    public Guid IdOutgoingWebhook { get; set; }

    public Guid IdAppUser { get; set; }

    public virtual AppUser IdAppUserNavigation { get; set; }

    public virtual OutgoingWebhook IdOutgoingWebhookNavigation { get; set; }
}
