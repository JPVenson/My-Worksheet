using System;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using MyWorksheet.Website.Shared.ViewModels.Notifications;

namespace MyWorksheet.Webpage.Services.WebHooks.WebhookTypes;

public class ProjectWebHook : IWebhookType
{
    public string Name { get; } = "Project";
    public Guid Id { get; set; } = new Guid("00000000-0000-0000-0001-000000000002");
    public object DefaultContent()
    {
        return new WebHookObject()
        {
            Type = ActionTypes.Created,
            Content = new GetProjectModel()
            {
                Name = "Example Project",
                IdOrganisation = Guid.Empty,
                //WorkTimeWednesday = 150,
                //WorkTimeSunday = -1,
                //WorkTimeTuesday = 150,
                //WorkTimeThursday = 150,
                //WorkTimeFriday = -1,
                //WorkTimeSaturday = -1,
                //WorkTimeMonday = 350,
                ProjectId = Guid.Empty,
            }
        };
    }

    public IWebHookObject CreateHookObject(GetProjectModel data, ActionTypes type)
    {
        return new WebHookObject()
        {
            Type = type,
            Content = data
        };
    }
}