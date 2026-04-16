using System;
using MyWorksheet.Website.Server.Models;

namespace MyWorksheet.Website.Server.Services.Activity.Types;

public class PaymentRecivedReminder : ActivityType
{
    public PaymentRecivedReminder() : base("payment-reminder-worksheet")
    {
    }

    public UserActivity CreateActivity(MyworksheetContext db, Worksheet worksheet, int inDays)
    {
        return new UserActivity()
        {
            ActivityType = TypeKey,
            DateCreated = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.Date.AddDays(inDays),
            SystemActivityTypeKey = worksheet.IdProject + "|" + worksheet.WorksheetId,
            HeaderHtml = "Payment Reminder for worksheet.",
            BodyHtml = "Did you recived the payment for the worksheet " + worksheet.StartTime.ToString("d") + " - " + worksheet.EndTime.Value.ToString("d"),
            FooterHtml = "{{goto:Links/Timeboard.Worksheet:WorksheetId=" + worksheet.WorksheetId + "}}",
            IdAppUser = worksheet.IdCreator
        };
    }
}