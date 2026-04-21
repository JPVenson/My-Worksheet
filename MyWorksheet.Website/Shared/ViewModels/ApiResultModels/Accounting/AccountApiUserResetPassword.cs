namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;

public class AccountApiUserResetPassword : ViewModelBase
{
    private string _confirmPassword;

    private string _newPassword;

    private string _recapture;

    private string _resetToken;

    public string ResetToken
    {
        get { return _resetToken; }
        set { SetProperty(ref _resetToken, value); }
    }

    public string Recapture
    {
        get { return _recapture; }
        set { SetProperty(ref _recapture, value); }
    }

    public string NewPassword
    {
        get { return _newPassword; }
        set { SetProperty(ref _newPassword, value); }
    }

    public string ConfirmPassword
    {
        get { return _confirmPassword; }
        set { SetProperty(ref _confirmPassword, value); }
    }
}