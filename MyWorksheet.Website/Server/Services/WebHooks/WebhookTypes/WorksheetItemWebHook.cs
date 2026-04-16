using System;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using MyWorksheet.Website.Shared.ViewModels.Notifications;

namespace MyWorksheet.Webpage.Services.WebHooks.WebhookTypes;

public class WorksheetItemWebHook : IWebhookType
{
    public string Name { get; } = "Worksheet Item";
    public Guid Id { get; set; } = new Guid("00000000-0000-0000-0001-000000000003");

    public IWebHookObject CreateHookObject(WorksheetItemModel data, ActionTypes type)
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
            Type = ActionTypes.Updated | ActionTypes.Created | ActionTypes.Deleted,
            Content = new WorksheetItemModel()
            {
            }
        };
    }
}

public class ActivityWebHook : IWebhookType
{
    public string Name { get; } = "Activitiy";
    public Guid Id { get; set; } = new Guid("00000000-0000-0000-0001-000000000004");

    public IWebHookObject CreateHookObject(UserActivityViewModel data, ActionTypes type)
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
            Type = ActionTypes.Created | ActionTypes.Deleted,
            Content = new UserActivityViewModel()
            {
            }
        };
    }
}