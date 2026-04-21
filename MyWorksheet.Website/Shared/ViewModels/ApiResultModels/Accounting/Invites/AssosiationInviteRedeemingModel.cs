using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Roles;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Invites;

public class AssosiationInviteRedeemingModel : ViewModelBase
{
    private AccountApiUserPost _account;

    private string _externalId;

    private UserToUserRoleViewModel _role;

    public string ExternalId
    {
        get { return _externalId; }
        set { SetProperty(ref _externalId, value); }
    }

    public UserToUserRoleViewModel Role
    {
        get { return _role; }
        set { SetProperty(ref _role, value); }
    }

    public AccountApiUserPost Account
    {
        get { return _account; }
        set { SetProperty(ref _account, value); }
    }
}