using System;
using System.Linq;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Services.ChargeRate;
using MyWorksheet.Website.Client.Services.Http;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Repository;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Client.Util.View;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Organisation;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Payment;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.UserWorkload;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Workflow;
using Microsoft.AspNetCore.Components;
using Morestachio.Formatter.Predefined.Accounting;

namespace MyWorksheet.Website.Client.Pages.TimeTracking.Projects;

public class EditProjectViewModel : ViewModelBase
{
    private ProjectItemRateViewModel _defaultRate;
    private OrganizationViewModel _organization;
    private EntityState<GetUserWorkloadViewModel> _projectWorkloadState;
    private WorksheetWorkflowModel _workflow;
    private WorksheetWorkflowDataMapViewModel _worksheetWorkflowDataMap;
    private bool _noModifications;
    private EntityState<GetProjectModel> _projectState;
    private PaymentInfoModel _paymentCondition;


    public EditProjectViewModel(EntityState<GetProjectModel> project,
        ICacheRepository<ProjectItemRateViewModel> projectItemRateCacheRepository,
        ChargeRateService chargeRateService,
        ICacheRepository<WorksheetModel> worksheetCacheRepository)
    {
        ProjectState = project;
        _chargeRateService = chargeRateService;
        if (ProjectState.ListState == EntityListState.Pristine)
        {
            var projId = Project.ProjectId;
            Rates = new FutureTrackedList<ProjectItemRateViewModel>(async () =>
            {
                return (await projectItemRateCacheRepository.Cache.FindBy(e => e.IdProject == projId,
                    e => (e as ProjectItemRateApiAccess).GetRatesForProject(Project.ProjectId))).ToArray();
            });
            Worksheets = new FutureList<WorksheetModel>(async () =>
            {
                return (await worksheetCacheRepository.Cache.FindBy(e => e.IdProject == projId,
                    e => (e as WorksheetApiAccess).GetByProject(Project.ProjectId))).ToArray();
            });
        }
        else
        {
            Rates = new TrackedList<ProjectItemRateViewModel>();
            Worksheets = new TrackedList<WorksheetModel>();
        }

        Worksheets.WhenLoaded(() =>
        {
            NoModifications = Worksheets.Any(f => f.IdCurrentStatus != Guid.Empty);
        });

        Rates.WhenLoaded(SendPropertyChanged);
        Rates.WhenLoadedOnce(() => DefaultRate = Rates.FirstOrDefault(e => e.ProjectItemRateId == Project.IdDefaultRate));
        Worksheets.WhenLoaded(SendPropertyChanged);
        _chargeRateService.Load();
    }

    private bool _showHidden;
    private readonly ChargeRateService _chargeRateService;

    public bool ShowHidden
    {
        get { return _showHidden; }
        set
        {
            if (SetProperty(ref _showHidden, value))
            {
                Worksheets.Reset();
                Worksheets.Load();
            }
        }
    }

    public bool NoModifications
    {
        get { return _noModifications; }
        set
        {
            SetProperty(ref _noModifications, value);
        }
    }

    public GetProjectModel Project
    {
        get { return ProjectState.Entity; }
    }

    public EntityState<GetProjectModel> ProjectState
    {
        get { return _projectState; }
        set
        {
            SetProperty(ref _projectState, value);
        }
    }

    public PaymentInfoModel PaymentCondition
    {
        get { return _paymentCondition; }
        set { SetProperty(ref _paymentCondition, value); }
    }

    public WorksheetWorkflowModel Workflow
    {
        get { return _workflow; }
        set
        {
            SetProperty(ref _workflow, value);
        }
    }

    public WorksheetWorkflowDataMapViewModel WorksheetWorkflowDataMap
    {
        get { return _worksheetWorkflowDataMap; }
        set
        {
            SetProperty(ref _worksheetWorkflowDataMap, value);
        }
    }

    public GetUserWorkloadViewModel ProjectWorkload
    {
        get
        {
            return ProjectWorkloadState?.Entity;
        }
    }

    public EntityState<GetUserWorkloadViewModel> ProjectWorkloadState
    {
        get { return _projectWorkloadState; }
        set
        {
            SetProperty(ref _projectWorkloadState, value);
        }
    }

    public OrganizationViewModel Organization
    {
        get { return _organization; }
        set
        {
            SetProperty(ref _organization, value);
            Project.IdOrganisation = value?.OrganisationId;
        }
    }

    public IFutureTrackedList<ProjectItemRateViewModel> Rates { get; set; }
    public IFutureList<WorksheetModel> Worksheets { get; set; }

    public ProjectItemRateViewModel DefaultRate
    {
        get { return _defaultRate; }
        set
        {
            SetProperty(ref _defaultRate, value);
        }
    }

    public void AddNewRate()
    {
        _chargeRateService.ChargeRates.WhenLoadedOnce(() =>
        {
            Rates.Add(new ProjectItemRateViewModel()
            {
                Name = "Rate",
                Rate = 0,
                TaxRate = 0,
                IdProject = Project.ProjectId,
                CurrencyType = WellKnownCurrencies.GetCurrentCurrency().IsoName,
                IdProjectChargeRate = DefaultRate?.IdProjectChargeRate ?? _chargeRateService.ChargeRates.First().ProjectChargeRateId,
                ProjectItemRateId = Guid.NewGuid()
            });
            if (Rates.Count == 1)
            {
                DefaultRate = Rates.First();
            }
        });
    }

    public void RemoveRate(ProjectItemRateViewModel rate)
    {
        if (Rates.Count == 1)
        {
            return;
        }

        Rates.Remove(rate);
        if (DefaultRate == rate)
        {
            DefaultRate = Rates.First();
        }
    }
}