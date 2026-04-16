using System;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Activity.Types;

namespace MyWorksheet.Website.Server.Services.Activity;

public class MailWorkflowFailedActivity : ActivityType
{
    /// <inheritdoc />
    public MailWorkflowFailedActivity() : base("workflow_mail_failed")
    {
    }

    public UserActivity CreateActivity(MyworksheetContext db, Worksheet worksheet, Project project, Guid executingUser, string reason)
    {
        return new UserActivity()
        {
            ActivityType = TypeKey,
            DateCreated = DateTime.UtcNow,
            SystemActivityTypeKey = Guid.NewGuid().ToString(),
            HeaderHtml = $"Sending Mail for Workflow Status change failed",
            BodyHtml = $"Creating or Sending the Status for Project {project.Name} - '{worksheet.StartTime:d} / {worksheet.EndTime:d}' changed mail has failed because: \r\n" + reason,
            IdAppUser = executingUser
        };
    }
}