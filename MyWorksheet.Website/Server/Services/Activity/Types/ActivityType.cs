using System;
using System.Text;
using MyWorksheet.Website.Server.Models;

namespace MyWorksheet.Website.Server.Services.Activity.Types;

public class ActivityType
{
    public ActivityType(string typeKey)
    {
        TypeKey = typeKey;
    }

    public string TypeKey { get; private set; }

    public virtual bool ActivityActivated(UserActivity userActivity)
    {
        //AccessElement<ActivityHubInfo>.Instance.SendActivityChanged(userActivity.IdAppUser, userActivity, ExternalActivityAction.Created);
        if (userActivity.IdCreator.HasValue)
        {
            //AccessElement<ActivityHubInfo>.Instance.SendActivityChanged(userActivity.IdCreator.Value, userActivity, ExternalActivityAction.Created);
        }
        return true;
    }

    public UserActivity CreateActivity(MyworksheetContext db, string reason, string header, Guid forUser)
    {
        return new UserActivity()
        {
            ActivityType = TypeKey,
            DateCreated = DateTime.UtcNow,
            SystemActivityTypeKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(reason)),
            HeaderHtml = header,
            BodyHtml = reason,
            IdAppUser = forUser
        };
    }
}