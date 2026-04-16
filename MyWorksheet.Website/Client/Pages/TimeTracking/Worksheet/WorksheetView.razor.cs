using System;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.ChargeRate;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Util;
using MyWorksheet.Website.Client.Services.Repository;
using MyWorksheet.Website.Client.Services.Signal;
using MyWorksheet.Website.Client.Services.UserWorkload;
using MyWorksheet.Website.Client.Services.Workflow;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Client.Util.View;
using MyWorksheet.Website.Shared.Services.Activation;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Workflow;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.TimeTracking.Worksheet;

public partial class WorksheetView
{
    [Inject]
    public HttpService HttpService { get; set; }
    [Inject]
    public UserWorkloadService UserWorkloadService { get; set; }
    [Inject]
    public WorkflowService WorkflowService { get; set; }
    [Inject]
    public WorkflowUiService WorkflowUiService { get; set; }
    [Inject]
    public ChargeRateService ChargeRateService { get; set; }

    [Parameter]
    public Guid WorksheetId { get; set; }

    [Inject]
    public ActivatorService ActivatorService { get; set; }
    [Inject]
    public ICacheRepository<GetProjectModel> ProjectsRepository { get; set; }
    [Inject]
    public ICacheRepository<WorksheetModel> WorksheetRepository { get; set; }

    public WorksheetEditViewModel Model { get; set; }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        var model = new WorksheetEditViewModel();

        model.Worksheet = await WorksheetRepository.Cache.Find(WorksheetId);
        model.Project = await ProjectsRepository.Cache.Find(model.Worksheet.IdProject);

        TrackBreadcrumb(BreadcrumbService.AddModuleLink("Links/Timeboard.Projects"));
        TrackBreadcrumb(BreadcrumbService.AddModuleLink(new LocalizableString("Links/Timeboard.Project.Title", model.Project.Name), new
        {
            ProjectId = model.Worksheet.IdProject
        }));

        await SetTitleAsync(new LocalizableString("Links/Timeboard.Worksheet.Title", model.Worksheet.NumberRangeEntry));

        model.WorksheetItems = new FutureTrackedList<WorksheetItemModel>(async () => ServerErrorManager.Eval(await HttpService.WorksheetItemApiAccess.GetByWorksheet(WorksheetId).AsTask()));

        model.WorkflowHistory = new FutureList<WorksheetStatusModel>(async () => ServerErrorManager.Eval(await HttpService.WorksheetHistoryApiAccess.GetHistory(WorksheetId).AsTask()));
        var chargeRatesTask = ChargeRateService.GetRatesForProject(model.Worksheet.IdProject);
        var workloadTask = UserWorkloadService.GetWorkloadForProjectOrDefault(model.Worksheet.IdProject).AsTask();
        model.WorkflowActions = new FutureList<WorksheetWorkflowStepViewModel>(async () =>
            ServerErrorManager.Eval(await HttpService.WorksheetWorkflowApiAccess.GetCurrentStepsForWorksheet(WorksheetId).AsTask()));
        await using (var tasks = new TaskList())
        {
            tasks.Add(chargeRatesTask);
            tasks.Add(workloadTask);
            tasks.Add(WorkflowService.Workflows.Load());
        }
        model.ChargeRates = chargeRatesTask.Result.Where(e => !e.Hidden).ToArray();
        model.Workload = workloadTask.Result;
        model.WorktimeMode = UserWorkloadService.Modes.First(e => e.Key == model.Workload.WorkTimeMode);
        model.Workflow = WorkflowService.GetWorkflow(model.Worksheet.IdWorksheetWorkflow);
        if (model.Workflow != null)
        {
            model.WorkflowDataSet = WorkflowService.GetWorkflowData(model.Worksheet.IdWorksheetWorkflow.Value)
                .FirstOrDefault(e => e.WorksheetWorkflowDataMapId == model.Worksheet.IdWorksheetWorkflowDataMap);
        }

        Track<WorksheetItemModel>((e) => InvokeAsync(() => Changed(e)));
        WhenChanged(model.WorkflowActions).ThenRefresh(this);
        await using (var itemTasks = new TaskList())
        {
            itemTasks.Add(model.WorksheetItems.Load());
            itemTasks.Add(model.WorkflowActions.Load());
        }
        await model.RebuildWorksheetItems();
        Model = model;
        CheckSubmitted();
        WhenChanged(WorkflowService).ThenRefresh(this).Then(CheckSubmitted);
    }

    private async Task Changed(EntityChangedEventArguments eventArguments)
    {
        var model = Model;
        switch (eventArguments.ChangeEventTypes)
        {
            case ChangeEventTypes.Removed:
                foreach (var eventArgumentsId in eventArguments.Ids)
                {
                    model.WorksheetItems.RemoveId(eventArgumentsId);
                }
                break;
            case ChangeEventTypes.Added:
                var toAdd = eventArguments.Ids.Where(e => model.WorksheetItems.All(f => f.WorksheetItemId != e)).ToArray();
                if (toAdd.Length == 1)
                {
                    var wsItem = await HttpService.WorksheetItemApiAccess.GetSingle(toAdd[0]);
                    model.WorksheetItems.Add(wsItem.Object);
                    model.WorksheetItems.State(wsItem.Object).SetPristine();
                }
                else if (toAdd.Length > 1)
                {
                    var items = await HttpService.WorksheetItemApiAccess.GetList(toAdd);
                    foreach (var worksheetItemModel in items.Object)
                    {
                        model.WorksheetItems.Add(worksheetItemModel);
                        model.WorksheetItems.State(worksheetItemModel).SetPristine();
                    }
                }
                break;
            case ChangeEventTypes.Changed:
                var toChange = eventArguments.Ids.Where(e => model.WorksheetItems.All(f => f.WorksheetItemId != e)).ToArray();
                if (toChange.Length == 1)
                {
                    var wsItem = await HttpService.WorksheetItemApiAccess.GetSingle(toChange[0]);
                    model.WorksheetItems.RemoveId(toChange[0]);
                    model.WorksheetItems.Add(wsItem.Object);
                    model.WorksheetItems.State(wsItem.Object).SetPristine();
                }
                else if (toChange.Length > 1)
                {
                    var items = await HttpService.WorksheetItemApiAccess.GetList(toChange);
                    foreach (var worksheetItemModel in items.Object)
                    {
                        model.WorksheetItems.RemoveId(worksheetItemModel.WorksheetItemId);
                        model.WorksheetItems.Add(worksheetItemModel);
                        model.WorksheetItems.State(worksheetItemModel).SetPristine();
                    }
                }
                break;
        }

        await model.RebuildWorksheetItems();
        Render();
    }

    void CheckSubmitted()
    {
        Model.Worksheet.Submitted = !WorkflowService.CheckForNoModifications(Model.Workflow?.WorksheetWorkflowId,
            Model.Worksheet.IdCurrentStatus.Value);
    }

    private async void InvokeWorkflowChangeTo(WorksheetWorkflowStepViewModel action)
    {
        ServerErrorManager.Clear();
        var (success, _) = await WorkflowUiService.StartChangeWorkflowStep(Model.Worksheet.WorksheetId, action.Value, Model.Worksheet.IdWorksheetWorkflow.Value, ServerErrorManager);
        if (!success)
        {
            if (ServerErrorManager.ServerErrors.Any())
            {
                ServerErrorManager.DisplayStatus();
            }
        }
        else
        {
            Model.WorkflowActions.Reset();
            Model.WorkflowHistory.Reset();
            await Model.WorkflowActions.Load();
            CheckSubmitted();
        }
    }
}