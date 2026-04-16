using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Storage;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Settings.Storage;

public partial class StorageListView
{
    public StorageListView()
    {

    }

    [Inject]
    public HttpService HttpService { get; set; }

    public IFutureList<GetStorageProvider> StorageProviders { get; set; }

    protected override void OnInitialized()
    {
        StorageProviders = new FutureList<GetStorageProvider>(async () => await HttpService.StorageApiAccess.GetStorageProviders());
        WhenChanged(StorageProviders).ThenRefresh(this);
        base.OnInitialized();
    }

    public void TryNavigate(GetStorageProvider provider)
    {
        if (provider.IdAppUser.HasValue)
        {
            NavigationService.NavigateTo("/Settings/Storage/" + provider.StorageProviderId);
        }
    }
}