using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class OutgoingWebhook : IUserRelation
{
    public Guid OutgoingWebhookId { get; set; }

    public string CallingUrl { get; set; }

    public Guid IdOutgoingWebhookCase { get; set; }

    public Guid IdCreator { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeactivated { get; set; }

    public string Secret { get; set; }

    public string NumberRangeEntry { get; set; }

    public string Name { get; set; }

    public virtual AppUser IdCreatorNavigation { get; set; }

    public virtual OutgoingWebhookCase IdOutgoingWebhookCaseNavigation { get; set; }

    public virtual ICollection<OutgoingWebhookActionLog> OutgoingWebhookActionLogs { get; set; } = [];
}
