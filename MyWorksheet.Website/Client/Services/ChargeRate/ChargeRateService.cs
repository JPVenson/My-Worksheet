using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Public.Models.Values;
using MyWorksheet.Website.Client.Services.ChangeTracking;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Repository;
using MyWorksheet.Website.Client.Services.Workflow;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Shared.Services.Activation;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using Morestachio.Formatter.Predefined.Accounting;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Client.Services.ChargeRate;

[SingletonService()]
public class ChargeRateService : LazyLoadedService
{
    private readonly HttpService _httpService;
    private readonly ChangeTrackingService _changeTrackingService;
    private readonly ICacheRepository<ProjectItemRateViewModel> _projectItemRateRepository;

    public ChargeRateService(HttpService httpService, ChangeTrackingService changeTrackingService,
        ICacheRepository<ProjectItemRateViewModel> projectItemRateRepository)
    {
        _httpService = httpService;
        _changeTrackingService = changeTrackingService;
        _projectItemRateRepository = projectItemRateRepository;
        ChargeRates = new FutureList<ProjectChargeRateModel>(() => _httpService.ProjectItemRateApiAccess.GetChargeRates().AsTask());
        ChargeRates.WhenLoadedOnce(OnDataLoaded);
    }

    public FutureList<ProjectChargeRateModel> ChargeRates { get; set; }

    public async Task<IEnumerable<ProjectItemRateViewModel>> GetRatesForProject(Guid projectId)
    {
        return await _projectItemRateRepository.Cache.FindBy(e => e.IdProject == projectId,
            (d) => _httpService.ProjectItemRateApiAccess.GetRatesForProject(projectId));
    }

    public ProjectChargeRateModel GetChargeRate(Guid id)
    {
        return ChargeRates.FirstOrDefault(e => e.ProjectChargeRateId == id);
    }

    public Money CalculateByTime(ProjectItemRateViewModel chargeRate, IGrouping<Guid, WorksheetItemModel> wsItems)
    {
        var rate = GetChargeRate(chargeRate.IdProjectChargeRate);
        if (rate == null)
        {
            return new Money(0, WellKnownCurrencies.EUR);
        }
        var time = wsItems.Select(f => new Worktime(f.ToTime - f.FromTime, WorktimePrecision.Minutes)).Aggregate((e, f) => e.Add(f.TimeWorked, f.Precision));
        return Money.Get(time, (double)chargeRate.Rate, rate.ToMRate(), WellKnownCurrencies.EUR);
    }

    public void Load()
    {
        ChargeRates.Load();
    }
}