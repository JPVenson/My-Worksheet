namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.UserSettings;

public class TimeOfDayThemeSwitch : ViewModelBase
{
    private int _fromTime;

    private string _theme;

    private int _toTime;

    public int FromTime
    {
        get { return _fromTime; }
        set { SetProperty(ref _fromTime, value); }
    }

    public int ToTime
    {
        get { return _toTime; }
        set { SetProperty(ref _toTime, value); }
    }

    public string Theme
    {
        get { return _theme; }
        set { SetProperty(ref _theme, value); }
    }
}