namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting
{
    public class TestUserData : ViewModelBase
    {
        private string _password;

        private string _username;

        public string Username
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }

        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }
    }
}