using System;

namespace MyWorksheet.Website.Shared.ViewModels.Notifications
{
    [Flags]
    public enum ActionTypes
    {
        Created,
        Updated,
        Submitted,
        Deleted
    }
}