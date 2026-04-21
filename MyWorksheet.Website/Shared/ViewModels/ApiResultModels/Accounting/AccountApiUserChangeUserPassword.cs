namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;

public class AccountApiUserChangeUserPassword : AccountApiUserChangePassword
{
    private string _username;

    public string Username
    {
        get { return _username; }
        set { SetProperty(ref _username, value); }
    }
}