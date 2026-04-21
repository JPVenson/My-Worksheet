using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Storage;

public class StorageProviderInfo : ViewModelBase
{
    private string _display;

    private string _key;

    public string Display
    {
        get { return _display; }
        set { SetProperty(ref _display, value); }
    }

    public string Key
    {
        get { return _key; }
        set { SetProperty(ref _key, value); }
    }
}

public class StorageTypeViewModel : ViewModelBase
{
    private Guid _storageTypeId;
    private string _name;

    public Guid StorageTypeId
    {
        get { return _storageTypeId; }
        set
        {
            SetProperty(ref _storageTypeId, value);
        }
    }

    public string Name
    {
        get { return _name; }
        set
        {
            SetProperty(ref _name, value);
        }
    }
}