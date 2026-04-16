using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Storage;
using Microsoft.AspNetCore.Components.Forms;

namespace MyWorksheet.Website.Client.Pages.Shared;

public class FileModel
{
    public FileModel(StorageEntryViewModel storageEntryViewModel)
    {
        Model = storageEntryViewModel;
    }

    public FileModel()
    {

    }

    public bool InTransition { get; set; }

    public IBrowserFile LocalFile { get; set; }

    public StorageEntryViewModel Model { get; set; }
    public bool ForceRefresh { get; set; }
}