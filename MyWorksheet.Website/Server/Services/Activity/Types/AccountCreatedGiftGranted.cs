using System;
using MyWorksheet.Website.Server.Models;

namespace MyWorksheet.Website.Server.Services.Activity.Types;

public class AccountCreatedGiftGranted : ActivityType
{
    public AccountCreatedGiftGranted() : base("account-created-gift")
    {
    }

    public UserActivity CreateActivity(MyworksheetContext db, AppUser appUser)
    {
        var address = db.Addresses.Find(appUser.IdAddress);
        return new UserActivity()
        {
            ActivityType = TypeKey,
            DateCreated = DateTime.UtcNow,
            HeaderHtml = "Welcome to My-Worksheet",
            BodyHtml = $"Hey ${address.FirstName}. Its better if its Free. My-Worksheet is Free now. Create as much Projects and Worksheets as you like to",
            IdAppUser = appUser.AppUserId
        };
    }
}