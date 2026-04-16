using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.ChangeTracking;
using MyWorksheet.Website.Client.Services.Dialog;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Storage;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Client.Util.View;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Storage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MyWorksheet.Website.Client.Pages.Shared;

public partial class FileListDisplay
{
    [Inject]
    public HttpService HttpService { get; set; }
    [Inject]
    public IJSRuntime IJSRuntime { get; set; }

    [Parameter]
    public IFutureList<FileModel> Files { get; set; }

    [Parameter]
    public Func<IFutureList<FileModel>> FilesFactory { get; set; }

    [Parameter]
    public EventCallback<FileModel> OnFileDeleteRequest { get; set; }

    [Inject]
    public ServerStorageService ServerStorageService { get; set; }

    [Inject]
    public DialogService DialogService { get; set; }

    protected override void OnInitialized()
    {
        Files = Files ?? FilesFactory();
        Track<StorageEntryViewModel>((state) =>
        {
            var model = Files.FirstOrDefault(f => state.Ids.Contains(f.Model.StorageEntryId));
            if (model != null)
            {
                model.ForceRefresh = true;
                Render();
            }
        });

        WhenChanged(ServerStorageService).ThenRefresh(this);

        base.OnInitialized();
    }

    public async Task DeleteFile(FileModel file)
    {
        if (file.Model == null)
        {
            Files.Remove(file);
            return;
        }

        file.InTransition = true;
        await OnFileDeleteRequest.RaiseAsync(file);
        Files.Remove(file);
        Render();
    }

    protected async void DownloadFile(FileModel file)
    {
        var tokenForStorageEntry = await HttpService.StorageApiAccess.CreateTokenForStorageEntry(file.Model.StorageEntryId);
        if (tokenForStorageEntry.Success)
        {
            var downloadUrl =
                HttpService.StorageApiAccess.DownloadUrl(file.Model.StorageEntryId, tokenForStorageEntry.Object);
            await IJSRuntime.InvokeVoidAsync("open", downloadUrl, "_blank");
        }
    }

    private void OpenLightbox(FileModel file)
    {
        var dialog = new FileDialogViewModel(Files, ServerStorageService);
        dialog.CurrentModel = file;
        DialogService.Show("FileLightbox", dialog, dialog, dialog);
    }
}