using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Public.Models.ObjectSchema;
using MyWorksheet.Website.Client.Components.Form;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Storage;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Client.Util.View;
using MyWorksheet.Website.Client.Util.View.List;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Storage;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Settings.Storage;

public partial class StorageEditView
{
    [Parameter]
    public Guid? Id { get; set; }
    [Inject]
    public HttpService HttpService { get; set; }
    [Inject]
    public ServerStorageService ServerStorageService { get; set; }

    public IFutureList<StorageProviderInfo> Provider { get; set; }
    public StorageProviderInfo SelectedProvider { get; set; }

    public EntityState<GetStorageProvider> StorageProvider { get; set; }
    public IObjectSchemaInfo ProviderDataSchema { get; set; }
    public ValueBag ProviderData { get; set; }

    public IEnumerable<string> TestResult { get; set; }

    public string SearchText { get; set; }
    public bool SearchDeleted { get; set; }
    public string SearchFileType { get; set; }

    public PagedList<StorageEntryViewModel> StorageEntries { get; set; }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        TrackBreadcrumb(BreadcrumbService.AddModuleLink("Links/Storage"));
        ProviderData = new ValueBag();
        Provider = ServerStorageService.Providers;
        WhenChanged(ServerStorageService).ThenRefresh(this);
        await Provider.Load();

        try
        {
            if (!Id.HasValue)
            {
                await SetTitleAsync(new LocalizableString("Links/Storage.Title", new LocalizableString("Common/New")));

                StorageProvider = new EntityState<GetStorageProvider>(new GetStorageProvider()
                {
                    Name = "Provider",
                    IsDefaultProvider = false,
                    StorageKey = Provider.FirstOrDefault()?.Key
                });
                return;
            }

            var data = ServerErrorManager.Eval(await HttpService.StorageApiAccess.GetStorageProvider(Id.Value).AsTask());
            if (!data.Success)
            {
                return;
            }

            await SetTitleAsync(new LocalizableString("Links/Storage.Title", data.Object.Name));

            StorageEntries = new PagedList<StorageEntryViewModel>(LoadStorageEntries, WaiterService);
            StorageEntries.PageSize = 7;
            WhenChanged(StorageEntries).ThenRefresh(this);


            StorageProvider = new EntityState<GetStorageProvider>(data.Object);
            var values = ServerErrorManager.Eval(await HttpService.StorageApiAccess.GetStorageProviderData(Id.Value));
            if (!values.Success)
            {
                return;
            }
            ProviderData.LoadWith(values.Object);
            await StorageEntries.SearchAsync();
            Render();
        }
        finally
        {

            SelectedProvider = Provider.FirstOrDefault(e => e.Key == StorageProvider.Entity.StorageKey) ??
                               Provider.First();
            ProviderDataSchema = await ServerStorageService.GetSchema(SelectedProvider.Key);
        }

    }

    private Task<ApiResult<PageResultSet<StorageEntryViewModel>>> LoadStorageEntries(PagedList<StorageEntryViewModel> arg)
    {
        return ServerErrorManager.Eval(HttpService.StorageApiAccess.GetStorageEntries(Id.Value, arg.Page, arg.PageSize, SearchDeleted, SearchText).AsTask());
    }

    public async Task SelectedProviderChanged(StorageProviderInfo provider)
    {
        ProviderDataSchema = await ServerStorageService.GetSchema(provider.Key);
    }

    public async Task Test()
    {
        ServerErrorManager.Clear();
        using (WaiterService.WhenDisposed())
        {
            var apiResult = ServerErrorManager
                .Eval(await HttpService.StorageApiAccess.TestProvider(SelectedProvider.Key, ProviderData.Values));
            if (apiResult.Success)
            {
                TestResult = apiResult.Object;
            }
        }
    }

    public async Task Delete()
    {
        ServerErrorManager.Clear();
        using (WaiterService.WhenDisposed())
        {
            var apiResult = ServerErrorManager.Eval(await HttpService.StorageApiAccess.Delete(StorageProvider.Entity.StorageProviderId));

            if (apiResult.Success)
            {
                NavigationService.NavigateTo("/Settings/Storage/");
            }
        }
    }

    public async Task Save()
    {
        ServerErrorManager.Clear();
        using (WaiterService.WhenDisposed())
        {
            if (Id.HasValue)
            {
                ServerErrorManager.Eval(await HttpService.StorageApiAccess.Update(new CreateStorageProvider()
                {
                    Fields = ProviderData.Values,
                    ProviderInfo = StorageProvider.Entity
                }, StorageProvider.Entity.StorageProviderId));
            }
            else
            {
                var apiResult = ServerErrorManager.Eval(await HttpService.StorageApiAccess.Create(new CreateStorageProvider()
                {
                    Fields = ProviderData.Values,
                    ProviderInfo = StorageProvider.Entity
                }));
                if (apiResult.Success)
                {
                    NavigationService.NavigateTo("/Settings/Storage/" + apiResult.Object.StorageProviderId);
                }
            }
            ServerErrorManager.DisplayStatus();
        }
    }
}