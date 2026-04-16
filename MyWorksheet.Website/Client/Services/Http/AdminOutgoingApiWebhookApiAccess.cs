using System;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Webhooks.Out;

namespace MyWorksheet.Website.Client.Services.Http;

public class AdminOutgoingApiWebhookApiAccess : HttpAccessBase
{
    public AdminOutgoingApiWebhookApiAccess(HttpService httpService) : base(httpService, "Webhook/OutgoingApi/Admin")
    {
    }

    public ValueTask<ApiResult<OutgoingWebhookModelGet[]>> Search(Guid userId)
    {
        return Get<OutgoingWebhookModelGet[]>(BuildApi("Webhooks", new
        {
            userId
        }));
    }

    public ValueTask<ApiResult<OutgoingWebhookModelGet>> Get(Guid id, Guid userId)
    {
        return Get<OutgoingWebhookModelGet>(BuildApi("Get", new { webhookId = id, userId }));
    }

    public ValueTask<ApiResult> Delete(Guid entityOutgoingWebhookId)
    {
        return Post(BuildApi("Delete", new
        {
            webhookId = entityOutgoingWebhookId
        }));
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

    public ValueTask<ApiResult<OutgoingWebhookModelGet>> Create(OutgoingWebhookModelGet model, Guid userId)
    {
        return Post<OutgoingWebhookModelGet, OutgoingWebhookModelGet>(BuildApi("Create", new
        {
            userId
        }), model);
    }

    public ValueTask<ApiResult<OutgoingWebhookModelGet>> Update(OutgoingWebhookModelGet model, Guid userId)
    {
        return Post<OutgoingWebhookModelGet, OutgoingWebhookModelGet>(BuildApi("Update", new
        {
            userId
        }), model);
    }
}