using System;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using MyWorksheet.Website.Shared.ViewModels.Notifications;

namespace MyWorksheet.Webpage.Services.WebHooks.WebhookTypes;

public class WorksheetWebHook : IWebhookType
{
    public string Name { get; } = "Worksheet";
    public Guid Id { get; set; } = new Guid("00000000-0000-0000-0001-000000000001");

    public IWebHookObject CreateHookObject(WorksheetModel data, ActionTypes type)
    {
        return new WebHookObject()
        {
            Type = type,
            Content = data
        };
    }

    public object DefaultContent()
    {
        return new WebHookObject()
        {
            Type = ActionTypes.Submitted | ActionTypes.Created | ActionTypes.Updated,
            Content = new WorksheetModel()
        };
    }
}