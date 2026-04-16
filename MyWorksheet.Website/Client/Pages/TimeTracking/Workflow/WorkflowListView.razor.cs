using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Repository;
using MyWorksheet.Website.Client.Services.Workflow;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Workflow;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.TimeTracking.Workflow;

public partial class WorkflowListView
{
    [Inject]
    public WorkflowService WorkflowService { get; set; }

    public FutureList<WorksheetWorkflowModel> Workflows { get; set; }
    public IDictionary<Guid, FutureList<WorksheetWorkflowDataMapViewModel>> WorkflowData { get; set; }

    /// <inheritdoc />
    public override async Task LoadDataAsync()
    {
        Workflows = WorkflowService.Workflows;
        WorkflowData = WorkflowService.WorkflowData;
        await Workflows.Load();
        await Task.WhenAll(Workflows.Select(f => WorkflowService.GetWorkflowData(f.WorksheetWorkflowId))
            .Select(f => f.Load()));
    }
}