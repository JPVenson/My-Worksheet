using System;
using System.ComponentModel.DataAnnotations;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Organisation
{
    public class OrganizationViewModel : UpdateOrganizationViewModel
    {
        private Guid _idAddress;

        private string _name;

        [Required]
        [Display(Name = "Entity/Organization")]
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public Guid IdAddress
        {
            get { return _idAddress; }
            set { SetProperty(ref _idAddress, value); }
        }
    }

    public class OrganisationReportingViewModel : OrganizationViewModel
    {
        public OrganizationViewModel ParentOrganization { get; set; }
    }
}