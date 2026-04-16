using System;
using MyWorksheet.Website.Server.Models;

namespace MyWorksheet.Website.Server.Services.Activity.Types;

public class TestAccountCreated : ActivityType
{
    public TestAccountCreated() : base("test-account-created")
    {
    }

    public UserActivity CreateActivity(MyworksheetContext db, AppUser appUser)
    {
        return new UserActivity()
        {
            ActivityType = TypeKey,
            DateCreated = DateTime.UtcNow,
            HeaderHtml = "New Test-Account created.",
            BodyHtml = "The User" + appUser.Email + " has created a new Test Account",
            IdAppUser = appUser.AppUserId
        };
    }
}