namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting
{
    public class AccountApiUserChangePassword : ViewModelBase
    {
        private string _confirmPassword;

        private string _newPassword;

        private string _oldPassword;

        public string OldPassword
        {
            get { return _oldPassword; }
            set { SetProperty(ref _oldPassword, value); }
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
}