using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Katana.CommonTasks.Models;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Workflow;

namespace MyWorksheet.Website.Server.Services.WorksheetWorkflowManagment;

public interface IWorksheetWorkflowStep
{
    IWorksheetStatusType StatusTypeId { get; }

    void Status(WorksheetWorkflowStatus status);

    Task<QuestionableBoolean> OnChange(MyworksheetContext db, Worksheet worksheet, Guid? fromState, IDictionary<string, object> additonalData);
    Task<QuestionableBoolean> AfterChange(MyworksheetContext db, Worksheet worksheet, Guid? fromState, Guid historyId, IDictionary<string, object> additonalData);

    Task<IObjectSchema> GetSchema(MyworksheetContext db, Guid userId, Guid worksheetId);
}
