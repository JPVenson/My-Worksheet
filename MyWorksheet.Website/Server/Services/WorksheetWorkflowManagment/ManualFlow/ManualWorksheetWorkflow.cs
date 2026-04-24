using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Katana.CommonTasks.Extentions;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Services.ExecuteLater;
using MyWorksheet.Webpage.Helper.Utlitiys;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater.ArgumentStore;
using MyWorksheet.Website.Server.Services.Reporting;
using MyWorksheet.Website.Server.Services.Reporting.Models;
using MyWorksheet.Website.Server.Services.Reporting.Text;
using MyWorksheet.Website.Server.Services.Workflow;
using MyWorksheet.Website.Server.Shared.Helper.ReportingDataSources;
using MyWorksheet.Website.Server.Shared.ObjectSchema;
using MyWorksheet.Website.Shared.Services.Activation;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Engine;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Reports;

namespace MyWorksheet.Website.Server.Services.WorksheetWorkflowManagment.ManualFlow;

public class ManualWorksheetWorkflow : IWorksheetWorkflow
{
    private readonly ActivatorService _activatorService;

    public ManualWorksheetWorkflow(ActivatorService activatorService)
    {
        _activatorService = activatorService;
        AllowedTransitions = new Dictionary<IWorksheetStatusType, IWorksheetStatusType[]>();

        WorksheetWorkflowSteps = new Dictionary<Guid, IWorksheetWorkflowStep>();
    }

    public void InitDone()
    {
        WorksheetWorkflowSteps[WorksheetStatusType.Created.ConvertToGuid()] = _activatorService.ActivateType<WorksheetCreatedStep>();
        WorksheetWorkflowSteps[WorksheetStatusType.Submitted.ConvertToGuid()] = _activatorService.ActivateType<WorksheetSubmitStep>();
        WorksheetWorkflowSteps[WorksheetStatusType.Confirmed.ConvertToGuid()] = _activatorService.ActivateType<WorksheetManualWorkflowConfirmedStep>();
    }

    public string ProviderKey { get; set; } = "ManualWorkflowImpl";

    public IWorksheetStatusType DefaultStep { get; set; }

    public bool NeedsCData { get; set; }

    public IWorksheetStatusType NoModifications { get; set; }

    public bool CanModify(IWorksheetStatusType step)
    {
        return step?.CompareTo(NoModifications) < 0;
    }

    public IDictionary<Guid, IWorksheetWorkflowStep> WorksheetWorkflowSteps { get; set; }
    public IDictionary<IWorksheetStatusType, IWorksheetStatusType[]> AllowedTransitions { get; set; }
    public IWorksheetStatusType[] AwaitingPaymentStep { get; set; } = new[] { WorksheetStatusType.Confirmed };
    public IWorksheetStatusType[] Closed { get; set; } = new[] { WorksheetStatusType.Payed };

    public IObjectSchema GetSchema(MyworksheetContext db, Guid userId)
    {
        var storageProviders = db.StorageProviders
            .Where(f => f.IdAppUser == null || f.IdAppUser == userId)
            .ToUniqeDictionary(e => e.Name, e => (object)e.StorageProviderId);

        var templatesOfTypeOfX = db.NengineTemplates
            .Where(e => e.IdCreator == null || e.IdCreator == userId)

            .ToArray()
            .Where(f => ReportDataSources.Yield().Where(e => e is IWorkflowTemplate)
                .Select(e => e.Key).Contains(f.UsedDataSource))
            .ToUniqeDictionary(e => e.Name, e => (object)e.NengineTemplateId);

        var schema = JsonSchemaExtensions.JsonSchema(typeof(ManualWorkflowData))
                .ExtendAllowedValues(
                    nameof(ManualWorkflowData.SubmitReport) + "." +
                    nameof(ManualWorkflowData.ManualWorkflowReport.StorageProvider), storageProviders)
                .ExtendAllowedValues(
                    nameof(ManualWorkflowData.SubmitReport) + "." +
                    nameof(ManualWorkflowData.ManualWorkflowReport.Template), templatesOfTypeOfX)
                .ExtendAllowedValues(
                    nameof(ManualWorkflowData.ConfirmedReport) + "." +
                    nameof(ManualWorkflowData.ManualWorkflowReport.StorageProvider), storageProviders)
                .ExtendAllowedValues(
                    nameof(ManualWorkflowData.ConfirmedReport) + "." +
                    nameof(ManualWorkflowData.ManualWorkflowReport.Template), templatesOfTypeOfX)
            ;

        var argumentSchema = ReportDataSources.WorksheetSpecReport.ArgumentSchema(db, userId) as JsonSchema;

        argumentSchema.Properties.Remove(nameof(WorkflowReportArguments.Worksheet));
        argumentSchema.Properties.Remove(nameof(WorkflowReportArguments.Date));
        schema.References["WorksheetSpecSchema"] = argumentSchema;
        schema.References[schema.Properties[nameof(ManualWorkflowData.SubmitReport)].Type.TypeName].Properties[nameof(ManualWorkflowData.ManualWorkflowReport.ReportData)] = new()
        {
            Name = "Workflow.Manual/Data.Comments.ReportData",
            Type = new()
            {
                TypeName = "WorksheetSpecSchema"
            }
        };
        schema.References[schema.Properties[nameof(ManualWorkflowData.ConfirmedReport)].Type.TypeName].Properties[nameof(ManualWorkflowData.ManualWorkflowReport.ReportData)] = new()
        {
            Name = "Workflow.Manual/Data.Comments.ReportData",
            Type = new()
            {
                TypeName = "WorksheetSpecSchema"
            }
        };

        return schema;
    }

    public static async Task<bool> RunAfterStatusChangeReport(MyworksheetContext db, Worksheet worksheet,
        Guid historyId,
        IDictionary<string, object> additonalData,
        string reportingPartName, ITextTemplateManager textTemplateManager)
    {
        if (!additonalData.ContainsKey(nameof(ManualWorkflowData.CreateReport)) ||
            !true.Equals(additonalData[nameof(ManualWorkflowData.CreateReport)])
            || !additonalData.ContainsKey(reportingPartName))
        {
            return true.Because("Ether the CreateReport flag was not set or the required Reporting information are incomplete");
        }

        var submitReportSteps = additonalData[reportingPartName] as IDictionary<string, object>;

        if (submitReportSteps == null)
        {
            return false.Because("The 'Submit-Report' Workflow Dataset is invalid.");
        }

        var storageProvider = submitReportSteps.TryGetValue(nameof(ManualWorkflowData.ManualWorkflowReport.StorageProvider), out var spRaw) ? (Guid?)spRaw : null;
        var templateId = submitReportSteps.TryGetValue(nameof(ManualWorkflowData.ManualWorkflowReport.Template), out var tiRaw) ? (Guid?)tiRaw : null;
        var templateData =
            submitReportSteps[nameof(ManualWorkflowData.ManualWorkflowReport.ReportData)] as IDictionary<string, object>;

        if (!storageProvider.HasValue || !templateId.HasValue || templateData == null)
        {
            return false.Because("The 'Submit-Report' Workflow Dataset is invalid.");
        }

        templateData[nameof(WorkflowReportArguments.Worksheet)] = worksheet.WorksheetId;

        var dataSource = new Dictionary<string, object>();
        var scheduleReportModel = new ScheduleReportModel()
        {
            Arguments = templateData,
            ParameterValues = new ReportingExecutionParameterValue[0],
            Preview = false,
            StorageProvider = storageProvider.Value,
            TemplateId = templateId.Value
        };
        var prepareReport = textTemplateManager.PrepareReport(db, scheduleReportModel, worksheet.IdCreator, dataSource);

        if (!prepareReport.Success)
        {
            return false.Because(prepareReport.Error);
        }

        var findTemplate = prepareReport.Object;

        var run = await textTemplateManager.GenerateTemplate(findTemplate.NengineTemplateId, findTemplate.Template,
            new TextTemplateDataQuery
            {
                Values = new SerializableObjectDictionary<string, object>(dataSource),
                DataSourceName = findTemplate.UsedDataSource,
                FollowUpAction = new[] { ExternalSchedulableTasks.ADD_CREATED_REPORT_FROM_WORKFLOW }
            }, PriorityManagerLevel.FireAndForget, worksheet.IdCreator, scheduleReportModel.StorageProvider.Value,
            scheduleReportModel.Preview, new Dictionary<string, object>()
            {
                {"HistoryId", historyId}
            });
        return run.Success.Because(run.Error);
    }
}