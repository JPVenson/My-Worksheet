using System;
using System.Collections.Generic;
using System.Linq;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Workflow;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Client.Services.Workflow;

[SingletonService()]
public class WorkflowService : LazyLoadedService
{
    private readonly HttpService _httpService;

    public WorkflowService(HttpService httpService)
    {
        _httpService = httpService;
        Workflows = new FutureList<WorksheetWorkflowModel>(() => _httpService.WorksheetWorkflowApiAccess.GetWorkflows().AsTask());
        Workflows.WhenLoadedOnce(base.OnDataLoaded);
        WorkflowStatus = new FutureList<WorksheetWorkflowStatusLookupViewModel>(() => _httpService.WorksheetHistoryApiAccess.GetLookups().AsTask());
        WorkflowStatus.WhenLoadedOnce(() =>
        {
            base.OnDataLoaded();
        });

        WorkflowData = new Dictionary<Guid, FutureList<WorksheetWorkflowDataMapViewModel>>();
        StatusColors = new Dictionary<Guid, string>()
        {
            // {Guid.Parse(""), "danger"},
            // {Guid.Parse(""), "secondary"},
            // {Guid.Parse(""), "info"},
            // {Guid.Parse(""), "info"},
            // {Guid.Parse(""), "warning"},
            // {Guid.Parse(""), "info"},
            // {Guid.Parse(""), "danger"},
            // {Guid.Parse(""), "success"},
        };
    }

    public FutureList<WorksheetWorkflowModel> Workflows { get; set; }
    public FutureList<WorksheetWorkflowStatusLookupViewModel> WorkflowStatus { get; set; }
    public IDictionary<Guid, FutureList<WorksheetWorkflowDataMapViewModel>> WorkflowData { get; set; }

    public FutureList<WorksheetWorkflowDataMapViewModel> GetWorkflowData(Guid workflowId)
    {
        if (!WorkflowData.TryGetValue(workflowId, out var data))
        {
            WorkflowData[workflowId] =
                data = new FutureList<WorksheetWorkflowDataMapViewModel>(() => _httpService.WorksheetWorkflowDataApiAccess.GetWorkflowData(workflowId).AsTask());
            data.WhenLoadedOnce(base.OnDataLoaded);
        }

        return data;
    }

    public bool CheckForNoModifications(Guid? workflowId, Guid currentStep)
    {
        return WorkflowStatus.FirstOrDefault(e => e.WorksheetStatusLookupId == currentStep)?.AllowModifications ?? true;
        // var workflow = GetWorkflow(workflowId);
        // if (workflow == null)
        // {
        // 	return true;
        // }
        // return workflow.IdNoModificationsStep == currentStep;
    }

    public FutureList<WorksheetWorkflowModel> GetWorkflows()
    {
        return Workflows;
    }

    public WorksheetWorkflowModel GetWorkflow(Guid? worksheetIdWorksheetWorkflow)
    {
        if (worksheetIdWorksheetWorkflow == null)
        {
            return null;
        }

        return Workflows.FirstOrDefault(e => e.WorksheetWorkflowId == worksheetIdWorksheetWorkflow.Value);
    }

    public WorksheetWorkflowStatusLookupViewModel GetWorkflowStatus(Guid? worksheetIdWorksheetWorkflow, Guid worksheetIdCurrentStatus)
    {
        return WorkflowStatus.FirstOrDefault(e => e.WorksheetStatusLookupId == worksheetIdCurrentStatus);
    }

    public IDictionary<Guid, string> StatusColors { get; set; }

    public string GetWorkflowStatusColor(string workflowCode, Guid statusCode)
    {
        if (StatusColors.TryGetValue(statusCode, out var color))
        {
            return color;
        }

        return "primary";
    }

    public string GetWorkflowStatusColor(string workflowCode, int statusValue)
    {
        return "primary";
    }
}