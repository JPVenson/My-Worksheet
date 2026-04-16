using System.ComponentModel.DataAnnotations;

namespace MyWorksheet.Website.Shared.ViewModels
{
    public class LoginFormModel : ViewModelBase
    {
        [Required]
        [Display(Name = "Name")]
        public string Account
        {
            get { return _account; }
            set { SetProperty(ref _account, value); }
        }

        private string _account;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Passwort")]
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        private string _password;

        public string RedictUrl
        {
            get { return _redictUrl; }
            set { SetProperty(ref _redictUrl, value); }
        }

        private string _redictUrl;
    }
}