using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting
{
    public class AccountApiAdminPost : ViewModelBase
    {
        private string _contactName;

        private string _email;

        private Guid? _idAddress;

        private bool _isActive;

        private bool _needPasswordReset;

        private string _username;

        public string Username
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set { SetProperty(ref _isActive, value); }
        }

        public string Email
        {
            get { return _email; }
            set { SetProperty(ref _email, value); }
        }

        public string ContactName
        {
            get { return _contactName; }
            set { SetProperty(ref _contactName, value); }
        }

        public bool NeedPasswordReset
        {
            get { return _needPasswordReset; }
            set { SetProperty(ref _needPasswordReset, value); }
        }

        public Guid? IdAddress
        {
            get { return _idAddress; }
            set { SetProperty(ref _idAddress, value); }
        }
    }
}