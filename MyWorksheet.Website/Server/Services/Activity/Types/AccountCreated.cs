using System;
using MyWorksheet.Website.Server.Models;

namespace MyWorksheet.Website.Server.Services.Activity.Types;

public class AccountCreated : ActivityType
{
    public AccountCreated() : base("account-created")
    {
    }

    public UserActivity CreateActivity(MyworksheetContext db, AppUser appUser)
    {
        return new UserActivity()
        {
            ActivityType = TypeKey,
            DateCreated = DateTime.UtcNow,
            SystemActivityTypeKey = Guid.NewGuid().ToString(),
            HeaderHtml = "New Account created.",
            BodyHtml = "The User" + appUser.Email + " has created a new Account",
            IdAppUser = appUser.AppUserId
        };
    }
}