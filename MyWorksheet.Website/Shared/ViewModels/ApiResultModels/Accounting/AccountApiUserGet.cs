namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;

public class AccountApiUserGet : ViewModelBase
{
    private PageSetting _pageSettings;

    private AccountApiUserGetInfo _userInfo;

    public AccountApiUserGetInfo UserInfo
    {
        get { return _userInfo; }
        set { SetProperty(ref _userInfo, value); }
    }

    public PageSetting PageSettings
    {
        get { return _pageSettings; }
        set { SetProperty(ref _pageSettings, value); }
    }
}