using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Webhook;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Webhooks.Out;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Settings.Hooks;

public partial class HttpHooksListView
{
    public IFutureList<OutgoingWebhookModelGet> Webhooks { get; set; }

    public HttpHooksListView()
    {
        Webhooks = new FutureList<OutgoingWebhookModelGet>(() => HttpService.WebhookApiAccess.OutgoingApiWebhookApiAccess.Search().AsTask());
    }

    [Inject]
    public HttpService HttpService { get; set; }
    [Inject]
    public WebhookService WebhookService { get; set; }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        WhenChanged(Webhooks).ThenRefresh(this);
        await Webhooks.Load();
    }
}