namespace MyWorksheet.Website.Shared.ViewModels.Login;

public class ChallangeRequestBody : ViewModelBase
{
    public string Username
    {
        get { return _username; }
        set { SetProperty(ref _username, value); }
    }

    private string _username;
    public string Recaptcha
    {
        get { return _recaptcha; }
        set { SetProperty(ref _recaptcha, value); }
    }

    private string _recaptcha;
}