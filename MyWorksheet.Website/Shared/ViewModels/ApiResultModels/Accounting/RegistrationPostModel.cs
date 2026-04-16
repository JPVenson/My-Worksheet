using System;
using System.ComponentModel.DataAnnotations;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Address;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting
{
    public class RegistrationPostModel : ViewModelBase
    {
        private AddressModel _address;

        private string _email;

        private string _password;

        private string _recapture;

        private Guid _regionId;

        private string _username;

        [Required]
        public string Email
        {
            get { return _email; }
            set { SetProperty(ref _email, value); }
        }

        [Required]
        public string Username
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }

        [Required]
        public string Recapture
        {
            get { return _recapture; }
            set { SetProperty(ref _recapture, value); }
        }

        [Required]
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        [Required]
        public Guid RegionId
        {
            get { return _regionId; }
            set { SetProperty(ref _regionId, value); }
        }

        [Required]
        public AddressModel Address
        {
            get { return _address; }
            set { SetProperty(ref _address, value); }
        }

        public override Guid? GetModelIdentifier()
        {
            return RegionId;
        }
    }
}