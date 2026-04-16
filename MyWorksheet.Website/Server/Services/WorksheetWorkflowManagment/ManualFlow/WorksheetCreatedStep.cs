using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Katana.CommonTasks.Models;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Workflow;

namespace MyWorksheet.Website.Server.Services.WorksheetWorkflowManagment.ManualFlow;

public class WorksheetCreatedStep : IWorksheetWorkflowStep
{
    public IWorksheetStatusType StatusTypeId { get; }
    public void Status(WorksheetWorkflowStatus status)
    {
        status.Submitted = false;
    }

    public async Task<QuestionableBoolean> OnChange(MyworksheetContext db, Worksheet worksheet, Guid? fromState, IDictionary<string, object> additonalData)
    {
        worksheet.ServiceDescription = null;
        worksheet.No = null;
        db.Update(worksheet);
        await Task.CompletedTask;
        return true;
    }

    public async Task<QuestionableBoolean> AfterChange(MyworksheetContext db, Worksheet worksheet, Guid? fromState, Guid historyId, IDictionary<string, object> additonalData)
    {
        await Task.CompletedTask;
        return true;
    }

    public Task<IObjectSchema> GetSchema(MyworksheetContext db, Guid userId, Guid worksheetId)
    {
        return Task.FromResult<IObjectSchema>(JsonSchema.EmptyNotNull);
    }
}