using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Katana.CommonTasks.Models;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Workflow;

namespace MyWorksheet.Website.Server.Services.WorksheetWorkflowManagment.TemplatedFlow.Steps;

public class TemplatedStepBase : IWorksheetWorkflowStep
{
    public TemplatedStepBase(IWorksheetStatusType statusTypeId)
    {
        StatusTypeId = statusTypeId;
    }

    /// <inheritdoc />
    public IWorksheetStatusType StatusTypeId { get; }

    /// <inheritdoc />
    public void Status(WorksheetWorkflowStatus status)
    {
    }

    /// <inheritdoc />
    public async Task<QuestionableBoolean> OnChange(MyworksheetContext db,
            Worksheet worksheet,
            Guid? fromState,
            IDictionary<string, object> additonalData)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc />
    public async Task<QuestionableBoolean> AfterChange(MyworksheetContext db,
            Worksheet worksheet,
            Guid? fromState,
            Guid historyId,
            IDictionary<string, object> additonalData)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc />
    public async Task<IObjectSchema> GetSchema(MyworksheetContext db,
            Guid userId,
            Guid worksheetId)
    {
        throw new System.NotImplementedException();
    }
}
