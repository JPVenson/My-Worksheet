using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Components.Dialog;
using MyWorksheet.Website.Client.Services.ChargeRate;
using MyWorksheet.Website.Client.Services.Dialog;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Organisation;
using MyWorksheet.Website.Client.Services.Repository;
using MyWorksheet.Website.Client.Services.Workflow;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Client.Util.View;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Payment;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Morestachio.Formatter.Predefined.Accounting;

namespace MyWorksheet.Website.Client.Pages.TimeTracking.Projects;

public class ProjectDeleteDialogViewModel : DialogViewModelBase
{
    public ProjectDeleteDialogViewModel(string projectName)
    {
        ProjectName = projectName;
    }

    public string ProjectName { get; }

    private string _confirmProjectName;

    public string ConfirmProjectName
    {
        get { return _confirmProjectName; }
        set { SetProperty(ref _confirmProjectName, value); }
    }
}

public partial class ProjectDetailEditComponent
{
    public ProjectDetailEditComponent()
    {
        Currencies = CurrencyHandler.DefaultHandler.Currencies.Values;
    }

    public IEnumerable<Currency> Currencies { get; set; }

    [Inject]
    public HttpService HttpService { get; set; }
    [Inject]
    public WorkflowService WorkflowService { get; set; }
    [Inject]
    public OrganizationService OrganizationService { get; set; }
    [Inject]
    public ChargeRateService ChargeRateService { get; set; }

    [Inject]
    public ICacheRepository<PaymentInfoModel> PaymentInfoRepository { get; set; }

    [Inject]
    public IServiceProvider ServiceProvider { get; set; }

    [Parameter]
    public EditProjectViewModel Project { get; set; }

    public EditContext EditContext { get; set; }

    public string ConfirmProjectDeleteName { get; set; }

    private bool _editProjectsInvalid;
    public bool EditProjectsInvalid
    {
        get { return _editProjectsInvalid; }
        set
        {
            SetProperty(ref _editProjectsInvalid, value, EditProjectsInvalidChanged);
        }
    }

    public EventCallback<bool> EditProjectsInvalidChanged { get; set; }

    private void EditContext_OnValidationStateChanged(object sender, ValidationStateChangedEventArgs e)
    {
        EditProjectsInvalid = EditContext.GetValidationMessages().Any();
    }

    public override async Task LoadDataAsync()
    {
        await PaymentInfoRepository.Cache.LoadAll();

        Project.PaymentCondition =
            PaymentInfoRepository.Cache.FirstOrDefault(e => e.PaymentInfoId == Project.Project.IdPaymentCondition);

        WhenChanged(Project, Project.Project)
            .ThenRefresh(this);
        EditContext ??= new EditContext(Project);
        EditContext.EnableDataAnnotationsValidation(ServiceProvider);
        EditContext.OnValidationStateChanged -= EditContext_OnValidationStateChanged;
        EditContext.OnValidationStateChanged += EditContext_OnValidationStateChanged;
    }

    public void PromptDeleteProject()
    {
        var dialogViewModel = new ProjectDeleteDialogViewModel(Project.Project.Name);
        DialogService.Show("ConfirmProjectDelete", null, dialogViewModel, dialogViewModel);
    }

    public async Task DeleteProject()
    {
        using (WaiterService.WhenDisposed())
        {
            ServerErrorManager.ServerErrors.Clear();
            var result = ServerErrorManager.Eval(await HttpService.ProjectApiAccess.Delete(Project.Project.ProjectId));
            ServerErrorManager.DisplayStatus();
            if (result.Success)
            {
                NavigationService.NavigateTo("/Project");
            }
        }
    }

    private async Task SaveProject(EditContext obj)
    {
        using (WaiterService.WhenDisposed())
        {
            ServerErrorManager.ServerErrors.Clear();
            var postObject = new PostProjectApiModel();

            if (Project.ProjectState.IsObjectDirty || Project.Project.ProjectId == Guid.Empty)
            {
                postObject.Project = Project.Project;
            }

            postObject.ChargeRates = Project.Rates
                .GetStates()
                .Select(e => e.AsApiEntity())
                .Where(e => e != null)
                .ToArray();

            if (postObject.Project == null && !postObject.ChargeRates.Any())
            {
                return;
            }

            if (Project.Project.ProjectId == Guid.Empty)
            {
                var updateOrCreate = await HttpService.ProjectApiAccess.Create(postObject);
                if (updateOrCreate.Success)
                {
                    NavigationService.NavigateTo($"/Project/{updateOrCreate.Object.ProjectId}#Info", true);
                    return;
                }
            }
            else
            {
                var updateOrCreate = HttpService.ProjectApiAccess.Update(postObject, Project.Project.ProjectId);
                Project.ProjectState = ServerErrorManager.Eval(await updateOrCreate).Object ?? Project.ProjectState;
                if (postObject.ChargeRates.Any())
                {
                    Project.Rates.Reset();
                    Project.Rates.AddRange(postObject.ChargeRates.Select(e => e.Entity));
                }
            }

            ServerErrorManager.DisplayStatus();
            Render();
        }
    }

    private void UnsaveProject(EditContext obj)
    {
        Console.WriteLine("Errors");
    }
}