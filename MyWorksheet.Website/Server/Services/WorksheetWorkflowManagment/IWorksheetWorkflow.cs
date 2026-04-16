using System;
using System.Collections.Generic;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Workflow;

namespace MyWorksheet.Website.Server.Services.WorksheetWorkflowManagment;

public interface IWorksheetWorkflow
{
    void InitDone();
    string ProviderKey { get; set; }
    IWorksheetStatusType DefaultStep { get; set; }
    bool NeedsCData { get; set; }

    bool CanModify(IWorksheetStatusType step);

    IDictionary<Guid, IWorksheetWorkflowStep> WorksheetWorkflowSteps { get; set; }
    IDictionary<IWorksheetStatusType, IWorksheetStatusType[]> AllowedTransitions { get; set; }
    IWorksheetStatusType[] AwaitingPaymentStep { get; set; }
    IWorksheetStatusType[] Closed { get; set; }

    IObjectSchema GetSchema(MyworksheetContext db, Guid userId);
}
