using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class OutgoingWebhookCase
{
    public Guid OutgoingWebhookCaseId { get; set; }

    public string Name { get; set; }

    public string DescriptionHtml { get; set; }

    public virtual ICollection<OutgoingWebhook> OutgoingWebhooks { get; set; } = [];
}
