using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;

public class AccountApiUserCreate : ViewModelBase
{
    private bool _needPasswordReset = true;

    private Guid _regionId;

    private string _username;

    private string _userPlainTextPassword;

    public string Username
    {
        get { return _username; }
        set { SetProperty(ref _username, value); }
    }

    public string UserPlainTextPassword
    {
        get { return _userPlainTextPassword; }
        set { SetProperty(ref _userPlainTextPassword, value); }
    }

    public Guid RegionId
    {
        get { return _regionId; }
        set { SetProperty(ref _regionId, value); }
    }

    public bool NeedPasswordReset
    {
        get { return _needPasswordReset; }
        set { SetProperty(ref _needPasswordReset, value); }
    }

    public override Guid? GetModelIdentifier()
    {
        return RegionId;
    }
}