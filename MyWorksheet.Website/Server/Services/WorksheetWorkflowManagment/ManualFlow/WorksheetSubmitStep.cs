using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Katana.CommonTasks.Extentions;
using Katana.CommonTasks.Models;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Reporting;
using MyWorksheet.Website.Server.Services.Workflow;
using MyWorksheet.Website.Server.Services.WorksheetWorkflowManagment.MailFlow;
using MyWorksheet.Website.Server.Shared.ObjectSchema;

namespace MyWorksheet.Website.Server.Services.WorksheetWorkflowManagment.ManualFlow;

public class WorksheetSubmitStep : IWorksheetWorkflowStep
{
    private ITextTemplateManager _textTemplateManager;

    public WorksheetSubmitStep(ITextTemplateManager textTemplateManager)
    {
        _textTemplateManager = textTemplateManager;
    }

    public IWorksheetStatusType StatusTypeId => WorksheetStatusType.Created;
    public void Status(WorksheetWorkflowStatus status)
    {
        status.Submitted = true;
    }

    public async Task<QuestionableBoolean> OnChange(MyworksheetContext db, Worksheet worksheet, Guid? fromState,
        IDictionary<string, object> additonalData)
    {
        if (!worksheet.EndTime.HasValue)
        {
            var oldestWsItem = db.WorksheetItems
                .Where(f => f.IdWorksheet == worksheet.WorksheetId)
                .OrderByDescending(e => e.DateOfAction)
                .FirstOrDefault();
            var endTime = oldestWsItem?.DateOfAction.Date ?? worksheet.StartTime;
            worksheet.EndTimeOffset = (short)endTime.Offset.TotalMinutes;
            worksheet.EndTime = endTime.ToUniversalTime();
        }

        //if (wsItems == 0)
        //{
        //	return false.Because("An empty worksheet cannot be submitted");
        //}

        if (!additonalData.ContainsKey("ProjectDescription"))
        {
            return false.Because("Please enter a Worksheet number");
        }
        var description = additonalData["ProjectDescription"]?.ToString();
        worksheet.ServiceDescription = description;

        //IoC.Resolve<OvertimeService>().AccountOvertime(worksheet);

        await Task.CompletedTask;
        return true;
    }

    public async Task<QuestionableBoolean> AfterChange(MyworksheetContext db, Worksheet worksheet, Guid? fromState, Guid historyId, IDictionary<string, object> additonalData)
    {
        return await ManualWorksheetWorkflow.RunAfterStatusChangeReport(db, worksheet, historyId, additonalData, nameof(ManualWorkflowData.SubmitReport), _textTemplateManager);
    }

    public Task<IObjectSchema> GetSchema(MyworksheetContext db, Guid userId, Guid worksheetId)
    {
        var currentWs = db.Worksheets.Find(worksheetId);

        var latestWorksheetOfProject = db.Worksheets
            .Where(f => f.IdProject == currentWs.IdProject)
            .Where(f => f.ServiceDescription != null && f.ServiceDescription != "")
            .Where(f => f.IdCurrentStatus != WorksheetStatusType.Invalid.ConvertToGuid() && f.IdCurrentStatus != WorksheetStatusType.Created.ConvertToGuid()) // TODO .Where(f => f.IdCurrentStatus > WorksheetStatusType.Created.ConvertToInt())
            .OrderBy(e => e.EndTime)
            .FirstOrDefault();

        var schema = JsonSchemaExtensions.JsonSchema(new WorksheetMailFlowSubmitStep.WorksheetMailFlowSubmitArguments
        {
            ProjectDescription = latestWorksheetOfProject?.ServiceDescription ?? "",
        });

        return Task.FromResult<IObjectSchema>(schema);
    }
}