using System;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Website.Client.Components.Form;
using MyWorksheet.Website.Client.Pages.TimeTracking.Worksheet.WorkflowSteps;
using MyWorksheet.Website.Client.Services.Dialog;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Repository;
using MyWorksheet.Website.Client.Util.View;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Workflow;

namespace MyWorksheet.Website.Client.Services.Workflow.Types;

public class DefaultWorkflowClientHandler : IWorkflowClientHandler
{
    private readonly WorkflowService _workflowService;
    private readonly HttpService _httpService;
    private readonly DialogService _dialogService;
    private readonly ICacheRepository<WorksheetModel> _worksheetRepository;
    private readonly ICacheRepository<GetProjectModel> _projectRepository;

    public DefaultWorkflowClientHandler(WorkflowService workflowService,
                                        HttpService httpService,
                                        DialogService dialogService,
                                        ICacheRepository<WorksheetModel> worksheetRepository,
                                        ICacheRepository<GetProjectModel> projectRepository)
    {
        _workflowService = workflowService;
        _httpService = httpService;
        _dialogService = dialogService;
        _worksheetRepository = worksheetRepository;
        _projectRepository = projectRepository;
    }

    /// <inheritdoc />
    public async ValueTask OnBeforeStatusChange(Guid worksheetId, Guid workflowStep)
    {
    }

    /// <inheritdoc />
    public async ValueTask<(bool, Guid)> OnStatusChange(Guid worksheetId, Guid workflowStep, ServerErrorManager errorManager)
    {
        await OnBeforeStatusChange(worksheetId, workflowStep);
        var workflowTransitionData =
            errorManager.Eval(await _httpService.WorksheetWorkflowApiAccess.GetDataForNextStep(worksheetId, workflowStep));

        if (!workflowTransitionData.Success)
        {
            errorManager.DisplayStatus();
            return (false, Guid.Empty);
        }

        var vm = await GetUiWorkflowStepDialogModel(worksheetId, workflowStep, workflowTransitionData.Object);

        if (!vm.WithStatus)
        {
            return (false, Guid.Empty);
        }

        var result = errorManager.Eval(await _httpService.WorksheetWorkflowApiAccess.SetStatus(vm.Values.Values,
                                                                                       worksheetId,
                                                                                       vm.NextStatus.Value,
                                                                                       vm.SubmitReason ?? "N/A"));

        if (!result.Success)
        {
            errorManager.DisplayStatus(new LocalizableString("Workflow/ConfirmStatus.ChangedToText", vm.NextStatus.Display.AsLocString()));

            return (false, Guid.Empty);
        }

        await OnAfterStatusChange(worksheetId, workflowStep);

        return (true, vm.NextStatus.Value);
    }

    private async Task<UiWorkflowStepDialogModel> GetUiWorkflowStepDialogModel(Guid worksheetId,
                                                                               Guid workflowStep,
                                                                               JsonSchema workflowTransitionData)
    {
        var stepData = _workflowService.WorkflowStatus.Find(workflowStep);
        var worksheet = await _worksheetRepository.Cache.Find(worksheetId);
        var project = await _projectRepository.Cache.Find(worksheet.IdProject);

        var vm = new UiWorkflowStepDialogModel(worksheet,
                                               project,
                                               new WorksheetWorkflowStepViewModel()
                                               {
                                                   Display = (await stepData).DisplayKey,
                                                   Value = workflowStep
                                               },
                                               workflowTransitionData);

        await _dialogService.Show("DisplayGenericWorkflowStep", vm, vm, vm)
                            .Await();

        return vm;
    }

    /// <inheritdoc />
    public async ValueTask OnAfterStatusChange(Guid worksheetId, Guid workflowStep)
    {
    }
}
