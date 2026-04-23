using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Katana.CommonTasks.Extentions;
using Katana.CommonTasks.Models;
using MyWorksheet.Helper;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Public.Models.ObjectSchema;
using MyWorksheet.Webpage.Helper.Utlitiys;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Activity;
using MyWorksheet.Website.Server.Services.NumberRangeService;
using MyWorksheet.Website.Server.Services.Reporting;
using MyWorksheet.Website.Server.Services.Workflow;
using MyWorksheet.Website.Server.Shared.ObjectSchema;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Services.WorksheetWorkflowManagment.ManualFlow;

public class WorksheetManualWorkflowConfirmedStepArguments : ArgumentsBase
{
    [JsonDisplayKey("Worksheet.Manual/Steps.Confirmed.Arguments.Comments.InvoiceNo")]
    public string InvoiceNr { get; set; }
    [JsonDisplayKey("Worksheet.Manual/Steps.Confirmed.Arguments.Comments.Reminder")]
    public int? CreatePaymentReminderInDays { get; set; }
    [JsonDisplayKey("Worksheet.Manual/Steps.Confirmed.Arguments.Comments.DueDate")]
    public bool UpdateDueDate { get; set; }
}

public class WorksheetManualWorkflowConfirmedStep : IWorksheetWorkflowStep
{
    private readonly IActivityService _activityService;
    private ITextTemplateManager _textTemplateManager;
    private readonly INumberRangeService _numberRangeService;

    public WorksheetManualWorkflowConfirmedStep(IActivityService activityService, ITextTemplateManager textTemplateManager, INumberRangeService numberRangeService)
    {
        _activityService = activityService;
        _textTemplateManager = textTemplateManager;
        _numberRangeService = numberRangeService;
    }

    public IWorksheetStatusType StatusTypeId { get; } = WorksheetStatusType.Submitted;
    public void Status(WorksheetWorkflowStatus status)
    {
    }

    public async Task<QuestionableBoolean> OnChange(MyworksheetContext db,
        Worksheet worksheet,
        Guid? fromState,
        IDictionary<string, object> additonalData)
    {
        //var validateSchema = GetSchema(db, worksheet.IdCreator, worksheet.WorksheetId).Validate(additonalData, db.Config);
        //if (validateSchema.Any())
        //{
        //    return false.Because(validateSchema.Select(e => e.Key + ":" + e.Value.ToString())
        //                                       .Aggregate((e, f) => e + Environment.NewLine + f));
        //}

        if (!additonalData.ContainsKey("InvoiceNr"))
        {
            return false.Because("Please enter a Worksheet number");
        }

        var nNr = additonalData["InvoiceNr"] as string;

        var hasNr = db.Worksheets.Where(f => f.IdCreator == worksheet.IdCreator).Where(f => f.No == nNr).FirstOrDefault();
        if (hasNr != null)
        {
            return false.Because("Duplicated Invoice No.");
        }

        var paymentReminderString = additonalData.GetOrNull("CreatePaymentReminderInDays")?.ToString();
        if (paymentReminderString != null && int.TryParse(paymentReminderString, out var paymentReminder))
        {
            await _activityService.CreateActivity(
                ActivityTypes.PaymentRecivedReminder.CreateActivity(db, worksheet, paymentReminder));
        }

        var proj = db.Projects
            .Where(f => f.ProjectId == worksheet.IdProject)
            .FirstOrDefault();

        var paymentCondition = db.PaymentInfos
            .Where(f => f.PaymentInfoId == proj.IdPaymentCondition).FirstOrDefault();

        var updateDueDate = additonalData.GetOrNull("UpdateDueDate")?.Equals(true) == true || !worksheet.InvoiceDueDate.HasValue;

        var dueDate = updateDueDate ? DateTimeOffset.Now.AddDays(paymentCondition.PaymentTarget.Value) : worksheet.InvoiceDueDate;

        await db.Worksheets
            .Where(f => f.WorksheetId == worksheet.WorksheetId)
            .ExecuteUpdateAsync(f => f.SetProperty(e => e.No, nNr).SetProperty(e => e.InvoiceDueDate, dueDate))
            .ConfigureAwait(false);
        return true;
    }

    public async Task<QuestionableBoolean> AfterChange(MyworksheetContext db,
        Worksheet worksheet,
        Guid? fromState,
        Guid historyId,
        IDictionary<string, object> additonalData)
    {
        return await ManualWorksheetWorkflow.RunAfterStatusChangeReport(db, worksheet, historyId, additonalData,
            nameof(ManualWorkflowData.ConfirmedReport), _textTemplateManager);
    }

    public async Task<IObjectSchema> GetSchema(MyworksheetContext db, Guid userId, Guid worksheetId)
    {
        var worksheet = db.Worksheets.Find(worksheetId);

        var lastActivitys = db.UserActivities
            .Where(f => f.ActivityType == ActivityTypes.PaymentRecivedReminder.TypeKey)
            .Where(f => f.IdAppUser == userId)
            .Where(f => f.SystemActivityTypeKey.StartsWith(worksheet.IdProject + "|"))
            .OrderBy(e => e.DateCreated)
            .Take(10)
            .ToArray();

        var average = 0;
        if (lastActivitys.Any())
        {
            average = (int)lastActivitys.Select(e => (e.DueDate.Value - e.DateCreated).Days).Average();
        }

        return JsonSchemaExtensions.JsonSchema(new WorksheetManualWorkflowConfirmedStepArguments
        {
            InvoiceNr = await _numberRangeService.GetNumberRangeAsync(db, InvoiceNumberRangeFactory.NrCode, userId, worksheet),
            CreatePaymentReminderInDays = new int?(average)
        });
    }
}