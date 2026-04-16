namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Organisation
{
    public class OrganizationReportingViewModel : OrganizationViewModel
    {
        private OrganizationViewModel _parentOrganization;

        public OrganizationViewModel ParentOrganization
        {
            get { return _parentOrganization; }
            set { SetProperty(ref _parentOrganization, value); }
        }
    }
}