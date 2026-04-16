using System.Collections.Generic;
using System.Linq;
using MyWorksheet.Website.Client.Components.Dialog;
using MyWorksheet.Website.Client.Services.Storage;

namespace MyWorksheet.Website.Client.Pages.Shared;

public class FileDialogViewModel : DialogViewModelBase
{
    public IList<FileModel> Model { get; }
    public FileModel CurrentModel { get; set; }

    public FileDialogViewModel(IList<FileModel> model, ServerStorageService serverStorageService)
    {
        Model = model;
        ServerStorageService = serverStorageService;
    }

    public ServerStorageService ServerStorageService { get; }

    public bool CanNextFile()
    {
        return Model.IndexOf(CurrentModel) + 1 <= Model.Count - 1;
    }

    public void NextFile()
    {
        if (!CanNextFile())
        {
            return;
        }

        CurrentModel = Model.ElementAt(Model.IndexOf(CurrentModel) + 1);
        SendPropertyChanged();
    }

    public bool CanPreviousFile()
    {
        return Model.IndexOf(CurrentModel) - 1 > 0;
    }

    public void PreviousFile()
    {
        if (!CanPreviousFile())
        {
            return;
        }

        CurrentModel = Model.ElementAt(Model.IndexOf(CurrentModel) - 1);
        SendPropertyChanged();
    }
}