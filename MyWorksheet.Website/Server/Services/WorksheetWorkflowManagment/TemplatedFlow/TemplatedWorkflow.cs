using System;
using System.Collections.Generic;
using System.Linq;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Webpage.Helper.Utlitiys;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Workflow;
using MyWorksheet.Website.Server.Shared.Helper.ReportingDataSources;
using MyWorksheet.Website.Server.Shared.ObjectSchema;

namespace MyWorksheet.Website.Server.Services.WorksheetWorkflowManagment.TemplatedFlow;

public class TemplatedWorkflow : IWorksheetWorkflow
{
    public TemplatedWorkflow()
    {
        AllowedTransitions = new Dictionary<IWorksheetStatusType, IWorksheetStatusType[]>();
        WorksheetWorkflowSteps = new Dictionary<Guid, IWorksheetWorkflowStep>();
    }

    /// <inheritdoc />
    public void InitDone()
    {

    }

    /// <inheritdoc />
    public string ProviderKey { get; set; } = "TemplatedWorkflow";

    /// <inheritdoc />
    public IWorksheetStatusType DefaultStep { get; set; }

    /// <inheritdoc />
    public IWorksheetStatusType NoModifications { get; set; }

    /// <inheritdoc />
    public bool NeedsCData { get; set; }

    /// <inheritdoc />
    public bool CanModify(IWorksheetStatusType step)
    {
        return step?.CompareTo(NoModifications) < 0;
    }

    /// <inheritdoc />
    public IDictionary<Guid, IWorksheetWorkflowStep> WorksheetWorkflowSteps { get; set; }

    /// <inheritdoc />
    public IDictionary<IWorksheetStatusType, IWorksheetStatusType[]> AllowedTransitions { get; set; }

    public IWorksheetStatusType[] AwaitingPaymentStep { get; set; } = new[]
    {
        WorksheetStatusType.AwaitingPayment
    };
    public IWorksheetStatusType[] Closed { get; set; } = new[]
    {
        WorksheetStatusType.Payed
    };

    /// <inheritdoc />
    public IObjectSchema GetSchema(MyworksheetContext db, Guid userId)
    {
        var knownReports = db.NengineTemplates.Where(e => e.IdCreator == null || e.IdCreator == userId)
                             .Where(f => f.UsedDataSource == ReportDataSources.WorksheetSpecReport.Key)
                             .Where(f => f.FileExtention == "pdf")
                             .ToArray()
                             .ToUniqeDictionary(e => e.Name, e => (object)e.NengineTemplateId);

        var knownStorageProvider = db.StorageProviders
                                     .Where(f => f.IdAppUser == null || f.IdAppUser == userId)
                                     .ToArray()
                                     .ToUniqeDictionary(e => e.Name, e => (object)e.StorageProviderId);

        return JsonSchemaExtensions.JsonSchema(typeof(TemplatedWorkflowData))
                                   .ExtendAllowedValues(nameof(TemplatedWorkflowData.ReportTemplateOnAwaitingPayment), knownReports)
                                   .ExtendAllowedValues(nameof(TemplatedWorkflowData.ReportTemplateOnAwaitingResponse), knownReports)
                                   .ExtendAllowedValues(nameof(TemplatedWorkflowData.ReportTemplateOnConfirmed), knownReports)
                                   .ExtendAllowedValues(nameof(TemplatedWorkflowData.ReportTemplateOnCreated), knownReports)
                                   .ExtendAllowedValues(nameof(TemplatedWorkflowData.ReportTemplateOnSubmitted), knownReports)
                                   .ExtendAllowedValues(nameof(TemplatedWorkflowData.StorageProvider), knownStorageProvider);
    }
}
