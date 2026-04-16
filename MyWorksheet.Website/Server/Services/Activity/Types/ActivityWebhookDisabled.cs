using System;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Activity.Types;

namespace MyWorksheet.Website.Server.Services.Activity;

public class ActivityWebhookDisabled : ActivityType
{
    public ActivityWebhookDisabled() : base("webhook_auto_disabled")
    {
    }

    public string FormatKey(OutgoingWebhook ws)
    {
        return ws.OutgoingWebhookId.ToString();
    }

    public UserActivity CreateActivity(MyworksheetContext db, OutgoingWebhook counter)
    {
        return new UserActivity()
        {
            ActivityType = TypeKey,
            DateCreated = DateTime.UtcNow,
            SystemActivityTypeKey = FormatKey(counter),
            HeaderHtml = "Webhook disabled",
            BodyHtml = "The webhook for Url: \"" + counter.CallingUrl + "\" was disabled because it reached its maximum of failed requests in a certain timespan",
            IdAppUser = counter.IdCreator
        };
    }
}