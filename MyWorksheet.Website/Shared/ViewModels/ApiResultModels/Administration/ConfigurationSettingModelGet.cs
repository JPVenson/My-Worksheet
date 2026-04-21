namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Administration;

public class ConfigurationSettingModelGet : ViewModelBase
{
    private string _fullOriginalName;

    private string _name;

    private bool _readOnly;

    private object _value;

    public string FullOriginalName
    {
        get { return _fullOriginalName; }
        set { SetProperty(ref _fullOriginalName, value); }
    }

    public string Name
    {
        get { return _name; }
        set { SetProperty(ref _name, value); }
    }

    public object Value
    {
        get { return _value; }
        set { SetProperty(ref _value, value); }
    }

    public bool ReadOnly
    {
        get { return _readOnly; }
        set { SetProperty(ref _readOnly, value); }
    }
}