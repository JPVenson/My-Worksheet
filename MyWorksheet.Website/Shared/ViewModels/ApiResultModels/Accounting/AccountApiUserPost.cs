using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting
{
    public class AccountApiUserPost : ViewModelBase
    {
        private bool _allowUpdates;

        private string _contactName;

        private string _email;

        private Guid _idAddress;

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

        public bool AllowUpdates
        {
            get { return _allowUpdates; }
            set { SetProperty(ref _allowUpdates, value); }
        }

        public Guid IdAddress
        {
            get { return _idAddress; }
            set { SetProperty(ref _idAddress, value); }
        }
    }
}