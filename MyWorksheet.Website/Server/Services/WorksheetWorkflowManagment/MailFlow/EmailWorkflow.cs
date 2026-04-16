using System;
using System.Collections.Generic;
using System.Linq;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Workflow;
using MyWorksheet.Website.Server.Shared.Helper.ReportingDataSources;
using MyWorksheet.Website.Server.Shared.ObjectSchema;
using MyWorksheet.Website.Shared.Services.Activation;

namespace MyWorksheet.Website.Server.Services.WorksheetWorkflowManagment.MailFlow;

public class EmailWorkflow : IWorksheetWorkflow
{
    private readonly ActivatorService _activatorService;

    public EmailWorkflow(ActivatorService activatorService)
    {
        _activatorService = activatorService;
        AllowedTransitions = new Dictionary<IWorksheetStatusType, IWorksheetStatusType[]>();
        WorksheetWorkflowSteps = new Dictionary<Guid, IWorksheetWorkflowStep>();
    }

    public void InitDone()
    {
        WorksheetWorkflowSteps[WorksheetStatusType.AwaitingResponse.ConvertToGuid()]
            = _activatorService.ActivateType<WorksheetMailFlowSubmitStep>(WorksheetStatusType.AwaitingResponse);
        //WorksheetWorkflowSteps[WorksheetStatusType.AwaitingPayment.ConvertToGuid()] = new WorksheetMailFlowSubmitStep(WorksheetStatusType.AwaitingPayment);
    }

    public string ProviderKey { get; set; } = "MailWorkflow";

    public IWorksheetStatusType DefaultStep { get; set; }
    public IWorksheetStatusType NoModifications { get; set; }

    public bool NeedsCData { get; set; }

    public bool CanModify(IWorksheetStatusType step)
    {
        return step?.CompareTo(NoModifications) < 0;
    }

    public IDictionary<Guid, IWorksheetWorkflowStep> WorksheetWorkflowSteps { get; set; }
    public IDictionary<IWorksheetStatusType, IWorksheetStatusType[]> AllowedTransitions { get; set; }
    public IWorksheetStatusType[] AwaitingPaymentStep { get; set; } = { WorksheetStatusType.AwaitingPayment };
    public IWorksheetStatusType[] Closed { get; set; } = { WorksheetStatusType.Payed };

    public IObjectSchema GetSchema(MyworksheetContext db, Guid userId)
    {
        var knownReports = db.NengineTemplates
                             .Where(e => e.IdCreator == null || e.IdCreator == userId)
                             .Where(f => f.UsedDataSource == ReportDataSources.WorksheetSpecReport.Key)
                             .Where(f => f.FileExtention == "html")
                             .ToArray()
                             .ToDictionary(e => e.Name, e => (object)e.NengineTemplateId);

        var knownStorageProvider = db.StorageProviders
                                    .Where(e => e.IdAppUser == null || e.IdAppUser == userId)
                                    .ToArray()
                                    .ToDictionary(e => e.Name, e => (object)e.StorageProviderId);

        var schema = JsonSchemaExtensions.JsonSchema(typeof(EMailWorkflowData))
                                         .ExtendAllowedValues(nameof(EMailWorkflowData.ReportTemplateAwaitingResponse), knownReports)
                                         .ExtendAllowedValues(nameof(EMailWorkflowData.MailStorageProvider), knownStorageProvider);

        return schema;
    }
}
