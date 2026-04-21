using System.Collections.Generic;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Administration;

public class ConfigurationModelGet : ViewModelBase
{
    private List<ConfigurationModelGet> _childs;

    private string _name;

    private string _path;

    private List<ConfigurationSettingModelGet> _settings;

    public string Path
    {
        get { return _path; }
        set { SetProperty(ref _path, value); }
    }

    public string Name
    {
        get { return _name; }
        set { SetProperty(ref _name, value); }
    }

    public List<ConfigurationSettingModelGet> Settings
    {
        get { return _settings; }
        set { SetProperty(ref _settings, value); }
    }

    public List<ConfigurationModelGet> Childs
    {
        get { return _childs; }
        set { SetProperty(ref _childs, value); }
    }
}