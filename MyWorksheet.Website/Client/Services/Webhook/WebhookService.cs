using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Workflow;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Shared.Services.Activation;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Webhooks.Out;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Client.Services.Webhook;

[SingletonService()]
public class WebhookService : LazyLoadedService
{
    private readonly HttpService _httpService;

    public WebhookService(HttpService httpService)
    {
        _httpService = httpService;
        WebhookTypes = new FutureList<OutgoingWebhookCaseModel>(() =>
            _httpService.WebhookApiAccess.OutgoingApiWebhookApiAccess.Types().AsTask());
    }

    public IFutureList<OutgoingWebhookCaseModel> WebhookTypes { get; set; }
}