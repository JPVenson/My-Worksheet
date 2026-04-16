using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Components.Form;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.WaiterIndicator;
using MyWorksheet.Website.Client.Services.Workflow;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Storage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace MyWorksheet.Website.Client.Pages.Shared;

public partial class FileListAdd
{
    public FileListAdd()
    {
        FilesToAdd = new List<BrowserFile>();
    }

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object> Attributes { get; set; }

    [Parameter]
    [Required]
    public Func<BrowserFile[], Guid, Task<FileModel[]>> OnFileAddRequest { get; set; }

    [Parameter]
    public IList<BrowserFile> FilesToAdd { get; set; }

    [Inject]
    public WaiterService WaiterService { get; set; }

    public ServerErrorManager ServerErrorManager { get; set; }

    public bool IsUploadingFiles { get; set; }

    [Inject]
    public StorageProviderService StorageProviderService { get; set; }

    private GetStorageProvider _selectedProvider;

    [Parameter]
    public GetStorageProvider SelectedProvider
    {
        get { return _selectedProvider; }
        set
        {
            SetProperty(ref _selectedProvider, value, SelectedProviderChanged);
        }
    }

    [Parameter]
    public EventCallback<GetStorageProvider> SelectedProviderChanged { get; set; }
    public string DropClass { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        StorageProviderService.StorageProviders.WhenLoadedOnce(() =>
        {
            SelectedProvider = StorageProviderService.StorageProviders.FirstOrDefault(e => e.IsDefaultProvider);
            StateHasChanged();
        });
        StorageProviderService.StorageProviders.Load();
        ServerErrorManager = new ServerErrorManager(WaiterService);
    }

    private async Task BeginUploadFiles()
    {
        if (SelectedProvider == null)
        {
            Console.WriteLine("No Provider Selected");
            return;
        }

        try
        {
            IsUploadingFiles = true;
            foreach (var browserFile in FilesToAdd.ToArray())
            {
                var model = await OnFileAddRequest(new BrowserFile[] { browserFile }, SelectedProvider.StorageProviderId);
                if (model != null)
                {
                    FilesToAdd.Remove(browserFile);
                    foreach (var fileModel in model)
                    {
                        Files.Add(fileModel);
                    }

                    StateHasChanged();
                }
            }
        }
        finally
        {
            IsUploadingFiles = false;
        }
    }
    private void HandleDragEnter()
    {
        DropClass = "dropAreaDrug";
    }
    private void HandleDragLeave()
    {
        DropClass = string.Empty;
    }

    private async Task FilesAdded(InputFileChangeEventArgs obj)
    {
        using (WaiterService.WhenDisposed())
        {
            foreach (var file in obj.GetMultipleFiles())
            {
                try
                {
                    using (var stream = file.OpenReadStream(HttpAccessBase.MaxFileSize))
                    {
                        using (var bufferStream = new MemoryStream())
                        {
                            await stream.CopyToAsync(bufferStream);
                            var thumbnailUrl = await IJSRuntime.InvokeAsync<string>(
                                "MyWorksheet.Blazor.CreateObjectURL",
                                bufferStream.ToArray(),
                                file.Name,
                                file.ContentType);
                            FilesToAdd.Add(new BrowserFile()
                            {
                                File = file,
                                CurrentProgress = 0,
                                MaxProgress = file.Size,
                                FileThumbnail = thumbnailUrl
                            });
                        }
                    }
                }
                catch (Exception e)
                {
                    ServerErrorManager.ServerErrors.Add(new ServerError()
                    {
                        ServerErrorText = new ServerProvidedTranslation()
                        {
                            Key = "{0}",
                            Arguments = new string[] { e.Message }
                        }
                    });
                }
            }
        }
    }
}