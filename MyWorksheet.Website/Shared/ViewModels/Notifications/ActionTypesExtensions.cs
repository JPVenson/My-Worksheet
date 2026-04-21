namespace MyWorksheet.Website.Shared.ViewModels.Notifications;

public static class ActionTypesExtensions
{
    public static bool HasFlagFast(this ActionTypes value, ActionTypes flag)
    {
        return (value & flag) != 0;
    }
}