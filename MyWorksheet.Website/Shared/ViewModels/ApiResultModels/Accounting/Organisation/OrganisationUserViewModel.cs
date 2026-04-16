using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Organisation
{
    public class OrganisationUserViewModel : ViewModelBase
    {
        private Guid _organisationId;

        private List<OrganizationMapViewModel> _toAdd;

        private List<OrganizationMapViewModel> _toRemove;

        public Guid OrganisationId
        {
            get { return _organisationId; }
            set { SetProperty(ref _organisationId, value); }
        }

        public List<OrganizationMapViewModel> ToAdd
        {
            get { return _toAdd; }
            set { SetProperty(ref _toAdd, value); }
        }

        public List<OrganizationMapViewModel> ToRemove
        {
            get { return _toRemove; }
            set { SetProperty(ref _toRemove, value); }
        }

        public override Guid? GetModelIdentifier()
        {
            return OrganisationId;
        }
    }
}