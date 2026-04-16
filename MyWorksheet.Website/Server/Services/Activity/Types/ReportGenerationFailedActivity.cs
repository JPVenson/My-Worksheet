using System;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Activity.Types;

namespace MyWorksheet.Website.Server.Services.Activity;

public class ReportGenerationFailedActivity : ActivityType
{
    public ReportGenerationFailedActivity() : base("WORKFLOW_ACTIVITY_FAILED")
    {
    }

    public UserActivity CreateActivity(MyworksheetContext db, string templateName, Guid executingUser)
    {
        return new UserActivity()
        {
            ActivityType = TypeKey,
            DateCreated = DateTime.UtcNow,
            SystemActivityTypeKey = Guid.NewGuid().ToString(),
            HeaderHtml = $"The creation of the Report for the Workflow has Failed",
            BodyHtml = $"The creation of the report {templateName} for a Workflow has failed. Please create and add the report manually",
            IdAppUser = executingUser
        };
    }
}