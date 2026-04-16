using System;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Organisation
{
    public class OrganisationRoleViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid Id { get; set; }
    }

    public class OrganizationSelectionViewModel : OrganizationViewModel
    {
        private Guid[] _idRelations;

        public Guid[] IdRelations
        {
            get { return _idRelations; }
            set { SetProperty(ref _idRelations, value); }
        }
    }
}