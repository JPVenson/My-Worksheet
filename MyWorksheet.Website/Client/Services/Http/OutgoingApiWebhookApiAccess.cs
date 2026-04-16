using System;
using System.Threading.Tasks;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Webhooks.Out;

namespace MyWorksheet.Website.Client.Services.Http;

public class OutgoingApiWebhookApiAccess : HttpAccessBase
{
    public OutgoingApiWebhookApiAccess(HttpService httpService) : base(httpService, "Webhook/OutgoingApi")
    {
        AdminApi = new AdminOutgoingApiWebhookApiAccess(httpService);
    }

    public AdminOutgoingApiWebhookApiAccess AdminApi { get; set; }

    public ValueTask<ApiResult<OutgoingWebhookModelGet[]>> Search()
    {
        return Get<OutgoingWebhookModelGet[]>(BuildApi("Webhooks"));
    }

    public ValueTask<ApiResult<JsonSchema>> GetTestData(Guid caseId)
    {
        return Get<JsonSchema>(BuildApi("ExampleCase", new { caseId }));
    }

    public ValueTask<ApiResult<OutgoingWebhookModelGet>> Get(Guid id)
    {
        return Get<OutgoingWebhookModelGet>(BuildApi("Get", new { webhookId = id }));
    }

    public ValueTask<ApiResult<PageResultSet<OutgoingWebhookActionLogModel>>> GetLog(Guid webhookId, int page, int size)
    {
        return Get<PageResultSet<OutgoingWebhookActionLogModel>>(BuildApi("Log", new
        {
            webhookId,
            page,
            size
        }));
    }

    public ValueTask<ApiResult<OutgoingWebhookModelGet>> Create(OutgoingWebhookModelGet model)
    {
        return Post<OutgoingWebhookModelGet, OutgoingWebhookModelGet>(BuildApi("Create"), model);
    }

    public ValueTask<ApiResult<OutgoingWebhookModelGet>> Update(OutgoingWebhookModelGet model)
    {
        return Post<OutgoingWebhookModelGet, OutgoingWebhookModelGet>(BuildApi("Update"), model);
    }

    public ValueTask<ApiResult<OutgoingWebhookCaseModel[]>> Types()
    {
        return Get<OutgoingWebhookCaseModel[]>(BuildApi("WebhookCase"));
    }

    public ValueTask<ApiResult> Delete(Guid entityOutgoingWebhookId)
    {
        return Post(BuildApi("Delete", new
        {
            webhookId = entityOutgoingWebhookId
        }));
    }
}