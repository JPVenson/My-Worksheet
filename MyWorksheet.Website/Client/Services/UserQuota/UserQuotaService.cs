using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.ChangeTracking;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Signal;
using MyWorksheet.Website.Client.Services.Workflow;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Shared.Services.Activation;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Client.Services.UserQuota;

[SingletonService()]
public class UserQuotaService : LazyLoadedService
{
    private readonly HttpService _httpService;
    private readonly ChangeTrackingService _changeTrackingService;

    public UserQuotaService(HttpService httpService, ChangeTrackingService changeTrackingService)
    {
        _httpService = httpService;
        _changeTrackingService = changeTrackingService;
        UserQuota = new FutureList<UserQuotaViewModel>(() => _httpService.AccountApiAccess.UserQuota().AsTask());
        UserQuota.WhenLoaded(OnDataLoaded);

        _changeTrackingService.RegisterTracking(typeof(UserQuotaViewModel), QuotaChanged);
    }

    private void QuotaChanged(EntityChangedEventArguments eventArguments)
    {
        UserQuota.Reset();
        OnDataLoaded();
    }

    public IFutureList<UserQuotaViewModel> UserQuota { get; set; }
}