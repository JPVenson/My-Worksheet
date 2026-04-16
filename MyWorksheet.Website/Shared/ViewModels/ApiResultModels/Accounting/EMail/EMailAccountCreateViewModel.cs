namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.EMail
{
    public class EMailAccountCreateViewModel : EMailAccountViewModel
    {
        private string _password;

        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }
    }
}