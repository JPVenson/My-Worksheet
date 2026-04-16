using System;
using System.ComponentModel.DataAnnotations;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Address
{
    public class AddressModel : UpdateAddressModel
    {
        private string _country;

        private DateTime _dateOfCreation;

        private string _firstName;

        private string _lastName;

        [Required]
        public string FirstName
        {
            get { return _firstName; }
            set { SetProperty(ref _firstName, value); }
        }

        [Required]
        public string LastName
        {
            get { return _lastName; }
            set { SetProperty(ref _lastName, value); }
        }

        [Required]
        public string Country
        {
            get { return _country; }
            set { SetProperty(ref _country, value); }
        }

        public DateTime DateOfCreation
        {
            get { return _dateOfCreation; }
            set { SetProperty(ref _dateOfCreation, value); }
        }
    }
}