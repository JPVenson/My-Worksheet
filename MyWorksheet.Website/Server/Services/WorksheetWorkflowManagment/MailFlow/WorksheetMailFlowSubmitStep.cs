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
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater.ArgumentStore;
using MyWorksheet.Website.Server.Services.Reporting;
using MyWorksheet.Website.Server.Services.Reporting.Text;
using MyWorksheet.Website.Server.Services.Workflow;
using MyWorksheet.Website.Server.Shared.ObjectSchema;
using MyWorksheet.ReportService.Services.ExecuteLater.Actions;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Services.WorksheetWorkflowManagment.MailFlow;

public class WorksheetMailFlowSubmitStep : IWorksheetWorkflowStep
{
    private readonly ITextTemplateManager _textTemplateManager;

    public WorksheetMailFlowSubmitStep(IWorksheetStatusType statusTypeId, ITextTemplateManager textTemplateManager)
    {
        _textTemplateManager = textTemplateManager;
        StatusTypeId = statusTypeId;
    }

    /// <inheritdoc />
    public IWorksheetStatusType StatusTypeId { get; private set; }

    /// <inheritdoc />
    public void Status(WorksheetWorkflowStatus status)
    {
        status.Submitted = true;
    }

    public class FlowStepArguments
    {
        public FlowStepArguments(IDictionary<string, object> additonalData)
        {
            ProjectDescription = additonalData.GetOrNull("ProjectDescription") as string;
            MailTemplateAwaitResponse = Guid.Parse(additonalData.GetOrNull(nameof(EMailWorkflowData.ReportTemplateAwaitingResponse))?.ToString() ?? Guid.Empty.ToString());
            MailStorageProvider = Guid.Parse(additonalData.GetOrNull(nameof(EMailWorkflowData.MailStorageProvider))?.ToString() ?? Guid.Empty.ToString());
        }

        public string ProjectDescription { get; private set; }
        public Guid MailTemplateAwaitResponse { get; set; }
        public Guid MailStorageProvider { get; set; }
    }

    /// <inheritdoc />
    public async Task<QuestionableBoolean> OnChange(MyworksheetContext db, Worksheet worksheet, Guid? fromState, IDictionary<string, object> additonalData)
    {
        var wsItems = db.WorksheetItems.Where(f => f.IdWorksheet == worksheet.WorksheetId).Count();
        if (wsItems == 0)
        {
            return false.Because("An empty worksheet cannot be submitted");
        }

        var arguments = new FlowStepArguments(additonalData);
        var project = db.Projects.Find(worksheet.IdProject);

        if (!project.IdOrganisation.HasValue)
        {
            return false.Because("To use the Mail-Workflow you have to set a Owner of this Project");
        }

        var findTemplate = db.NengineTemplates.Find(arguments.MailTemplateAwaitResponse);
        if (findTemplate.IdCreator.HasValue && findTemplate.IdCreator.Value != worksheet.IdCreator)
        {
            return false.Because("Could not find a Proper mail Template you defined.");
        }

        var createReportArguments = additonalData["Report"] as IDictionary<string, object>;
        createReportArguments["preId"] = fromState;

        var run = await _textTemplateManager.GenerateTemplate(findTemplate.NengineTemplateId, findTemplate.Template, new TextTemplateDataQuery()
        {
            Values = new SerializableObjectDictionary<string, object>(createReportArguments),
            FollowUpAction = new string[]
            {
                MailWorkfowQueueAction.ActionKey
            },
            DataSourceName = findTemplate.UsedDataSource,
            GeneratedQuery = ""
        }, PriorityManagerLevel.FireAndForget, worksheet.IdCreator, arguments.MailStorageProvider, false);

        if (!run.Success)
        {
            return false.Because(run.Error);
        }

        db.Worksheets.Where(e => e.WorksheetId == worksheet.WorksheetId)
            .ExecuteUpdate(f => f.SetProperty(e => e.ServiceDescription, arguments.ProjectDescription));

        await Task.CompletedTask;
        return true;
    }

    public async Task<QuestionableBoolean> AfterChange(MyworksheetContext db, Worksheet worksheet, Guid? fromState, Guid historyId, IDictionary<string, object> additonalData)
    {
        await Task.CompletedTask;
        return true;
    }

    public class WorksheetMailFlowSubmitArguments : ArgumentsBase
    {
        [JsonComment("Workflow.Manual/Step.Submit.Arguments.Comments.Description")]
        [JsonDisplayKey("Workflow.Manual/Step.Submit.Arguments.Names.Description")]
        public string ProjectDescription { get; set; }
    }

    /// <inheritdoc />
    public Task<IObjectSchema> GetSchema(MyworksheetContext db, Guid userId, Guid worksheetId)
    {
        var currentWs = db.Worksheets.Find(worksheetId);

        var latestWorksheetOfProject = db.Worksheets
            .Where(f => f.IdProject == currentWs.IdProject)
            .Where(f => f.ServiceDescription != null && f.ServiceDescription != "")
            .Where(f => f.IdCurrentStatus != WorksheetStatusType.Invalid.ConvertToGuid() && f.IdCurrentStatus != WorksheetStatusType.Created.ConvertToGuid())
            .OrderBy(e => e.EndTime)
            .FirstOrDefault();
        return
            Task.FromResult<IObjectSchema>(JsonSchemaExtensions.JsonSchema(new WorksheetMailFlowSubmitArguments
            {
                ProjectDescription = latestWorksheetOfProject?.ServiceDescription ?? "",
            }));
    }
}