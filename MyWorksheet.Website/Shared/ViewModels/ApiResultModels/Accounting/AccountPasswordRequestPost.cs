namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;

public class AccountPasswordRequestPost : ViewModelBase
{
    private string _email;

    private string _recapture;

    private string _username;

    public string Recapture
    {
        get { return _recapture; }
        set { SetProperty(ref _recapture, value); }
    }

    public string Username
    {
        get { return _username; }
        set { SetProperty(ref _username, value); }
    }

    public string Email
    {
        get { return _email; }
        set { SetProperty(ref _email, value); }
    }
}