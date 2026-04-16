using System;

namespace MyWorksheet.Website.Shared.ViewModels.Notifications
{
    public interface IWebhookType
    {
        string Name { get; }
        Guid Id { get; set; }

        object DefaultContent();
    }
}