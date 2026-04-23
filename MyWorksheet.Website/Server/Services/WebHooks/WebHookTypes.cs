using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Webpage.Services.WebHooks.WebhookTypes;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Shared.ViewModels.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ServiceLocator.Attributes;
using MyWorksheet.Website.Shared.Services;

namespace MyWorksheet.Webpage.Services.WebHooks;

[SingletonService]
public class WebHookTypes : RequireInit
{
    public static readonly ProjectWebHook Project = new ProjectWebHook();
    public static readonly WorksheetWebHook Worksheet = new WorksheetWebHook();
    public static readonly WorksheetItemWebHook WorksheetItem = new WorksheetItemWebHook();
    public static readonly ActivityWebHook ActivityWebHook = new ActivityWebHook();

    public static IEnumerable<IWebhookType> YieldTypes()
    {
        yield return Project;
        yield return Worksheet;
        yield return WorksheetItem;
        yield return ActivityWebHook;
    }

    public override async ValueTask InitAsync(IServiceProvider services)
    {
        await using var db = await services.GetRequiredService<IDbContextFactory<MyworksheetContext>>().CreateDbContextAsync().ConfigureAwait(false);
        var webhooks = await db.OutgoingWebhookCases.ToArrayAsync().ConfigureAwait(false);
        foreach (var item in webhooks.Join(YieldTypes(), e => e.Name, e => e.Name, (e, f) => (Id: e.OutgoingWebhookCaseId, WebhookType: f)))
        {
            item.WebhookType.Id = item.Id;
        }
    }
}