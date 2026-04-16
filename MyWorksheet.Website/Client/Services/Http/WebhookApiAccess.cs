using System;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;

namespace MyWorksheet.Website.Client.Services.Http;

public class WebhookApiAccess : HttpAccessBase
{
    public WebhookApiAccess(HttpService httpService) : base(httpService, "Webhook")
    {
        OutgoingApiWebhookApiAccess = new OutgoingApiWebhookApiAccess(httpService);
    }

    public OutgoingApiWebhookApiAccess OutgoingApiWebhookApiAccess { get; set; }
}

public class ActivityApiAccess : RestHttpAccessBase<UserActivityViewModel>
{
    public ActivityApiAccess(HttpService httpService) : base(httpService, "ActivityApi")
    {

    }

    public ValueTask<ApiResult<UserActivityViewModel[]>> GetActivities()
    {
        return Get<UserActivityViewModel[]>(BuildApi("GetUserActivities", new
        {
            showHidden = false
        }));
    }

    public ValueTask<ApiResult> HideActivity(Guid activityId)
    {
        return Post(BuildApi("HideActivity", new
        {
            activityId = activityId
        }));
    }
}