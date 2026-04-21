using System;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Administration;

public class ClientSettingsModel : ViewModelBase
{
    private MaintainceModeViewModel _maintainceModeUpdate;

    private string _realm;

    private string _recaptchaKey;

    private Version _version;

    private string _versionType;

    public string Realm
    {
        get { return _realm; }
        set { SetProperty(ref _realm, value); }
    }

    public string VersionType
    {
        get { return _versionType; }
        set { SetProperty(ref _versionType, value); }
    }

    public Version Version
    {
        get { return _version; }
        set { SetProperty(ref _version, value); }
    }

    public string RecaptchaKey
    {
        get { return _recaptchaKey; }
        set { SetProperty(ref _recaptchaKey, value); }
    }

    public MaintainceModeViewModel MaintainceModeUpdate
    {
        get { return _maintainceModeUpdate; }
        set { SetProperty(ref _maintainceModeUpdate, value); }
    }
}