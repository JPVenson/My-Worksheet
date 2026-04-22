using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Website.Client.Components.Form;
using MyWorksheet.Website.Client.Util;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Workflow;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Workflow;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.TimeTracking.Workflow;

public partial class WorkflowEditView
{
    [Parameter]
    public Guid WorkflowId { get; set; }
    [Parameter]
    public Guid? WorkflowDataId { get; set; }

    [Inject]
    public WorkflowService WorkflowService { get; set; }

    [Inject]
    public HttpService HttpService { get; set; }

    public WorksheetWorkflowModel WorksheetWorkflowModel { get; set; }
    public EntityState<WorksheetWorkflowDataMapViewModel> WorkflowDataViewModel { get; set; }
    public WorkflowDataViewModel DataSchema { get; set; }

    public ValueBag ValueBag { get; set; }

    /// <inheritdoc />
    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        TrackBreadcrumb(BreadcrumbService.AddModuleLink("Links/Timeboard.Workflow"));
        WorksheetWorkflowModel = await WorkflowService.Workflows.Find(WorkflowId);
        var workflowStructureTask = (HttpService.WorksheetWorkflowDataApiAccess.GetWorkflowStructure(WorkflowId));

        if (!WorkflowDataId.HasValue)
        {
            WorkflowDataViewModel = new EntityState<WorksheetWorkflowDataMapViewModel>(new WorksheetWorkflowDataMapViewModel()
            {
                Fields = new Dictionary<string, object>(),
                GroupKey = "",
                IdWorksheetWorkflow = WorkflowId
            });
            ValueBag = new();
        }
        else
        {
            var dataViewModelTask = WorkflowService.GetWorkflowData(WorkflowId).Find(WorkflowDataId.Value);
            var dataValuesTask = HttpService.WorksheetWorkflowDataApiAccess.GetValues(WorkflowDataId.Value).AsTask();
            await using (var tasks = new TaskList())
            {
                tasks.Add(dataViewModelTask);
                tasks.Add(dataValuesTask);
            }
            WorkflowDataViewModel = dataViewModelTask.Result;
            ValueBag = new ValueBag((await dataValuesTask).UnpackOrThrow().Object.Values);
        }
        DataSchema = (await workflowStructureTask).UnpackOrThrow().Object;

        //ValueBag.LoadWith(dataValues.Values);
    }

    public async Task SaveDataAsync()
    {
        using (WaiterService.WhenDisposed())
        {
            WorkflowDataViewModel.Entity.Fields = ValueBag.Values;

            if (WorkflowDataViewModel.ListState == EntityListState.Added)
            {
                var apiResult = await ServerErrorManager.Eval(HttpService.WorksheetWorkflowDataApiAccess.Create(WorkflowDataViewModel.Entity).AsTask());

                if (apiResult.Success)
                {
                    ModuleService.NavigateTo($"/Workflows/{WorkflowId}/{apiResult.Object.WorksheetWorkflowDataMapId}");
                    return;
                }
            }
            else
            {
                await ServerErrorManager.Eval(HttpService.WorksheetWorkflowDataApiAccess.Update(WorkflowDataViewModel.Entity).AsTask());
            }

            ServerErrorManager.DisplayStatus();
        }
    }
}