using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Storage;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Client.Services.Workflow;

[SingletonService()]
public class StorageProviderService : LazyLoadedService
{
    private readonly HttpService _httpService;

    public StorageProviderService(HttpService httpService)
    {
        _httpService = httpService;
        StorageProvidersTypes = new FutureList<StorageProviderInfo>(async () => await _httpService.StorageApiAccess.GetStorageProviderTypes());
        StorageProvidersTypes.WhenLoaded(OnDataLoaded);

        StorageProviders = new FutureList<GetStorageProvider>(async () =>
                                                                  await _httpService.StorageApiAccess.GetStorageProviders());
        StorageProviders.WhenLoaded(OnDataLoaded);
    }

    public IFutureList<StorageProviderInfo> StorageProvidersTypes { get; set; }
    public IFutureList<GetStorageProvider> StorageProviders { get; set; }
}