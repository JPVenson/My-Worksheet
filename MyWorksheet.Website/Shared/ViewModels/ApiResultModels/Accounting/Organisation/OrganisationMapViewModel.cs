using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Organisation
{
    public class OrganizationMapViewModel : UpdateOrganizationMapViewModel
    {
        private AccountApiUserGetInfo _appUser;
        public AccountApiUserGetInfo AppUser
        {
            get { return _appUser; }
            set { SetProperty(ref _appUser, value); }
        }
    }

    public class UpdateOrganizationMapViewModel : ViewModelBase
    {
        private Guid _idAppUser;

        private Guid _idRelation;

        public Guid IdAppUser
        {
            get { return _idAppUser; }
            set { SetProperty(ref _idAppUser, value); }
        }

        public Guid IdRelation
        {
            get { return _idRelation; }
            set { SetProperty(ref _idRelation, value); }
        }
    }
}