namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting
{
    public class TestUserAccountCreate : ViewModelBase
    {
        private string _mailAddress;

        private string _recapture;

        public string Recapture
        {
            get { return _recapture; }
            set { SetProperty(ref _recapture, value); }
        }

        public string MailAddress
        {
            get { return _mailAddress; }
            set { SetProperty(ref _mailAddress, value); }
        }
    }
}