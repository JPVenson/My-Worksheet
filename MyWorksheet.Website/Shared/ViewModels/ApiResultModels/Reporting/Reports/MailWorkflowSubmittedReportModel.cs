using MyWorksheet.Public.Models.ObjectSchema;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Organisation;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Address;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Reports
{
    public class MailWorkflowSubmittedReportModel : ViewModelBase
    {
        private string _acceptUrl;

        private string _additonalInfos;

        private AccountApiGet _creator;

        private AddressModel _creatorAddress;

        private OrganizationReportingViewModel _owner;

        private AddressModel _ownerAddress;

        private GetProjectModel _project;

        private string _rejectUrl;

        private WorksheetModel _worksheet;

        public string AcceptUrl
        {
            get { return _acceptUrl; }
            set { SetProperty(ref _acceptUrl, value); }
        }

        public string RejectUrl
        {
            get { return _rejectUrl; }
            set { SetProperty(ref _rejectUrl, value); }
        }

        public string AdditonalInfos
        {
            get { return _additonalInfos; }
            set { SetProperty(ref _additonalInfos, value); }
        }

        [JsonComment("Reporting/ProjectSpec.Comment.Owner")]
        public OrganizationReportingViewModel Owner
        {
            get { return _owner; }
            set { SetProperty(ref _owner, value); }
        }

        [JsonComment("Reporting/ProjectSpec.Comment.Creator")]
        public AccountApiGet Creator
        {
            get { return _creator; }
            set { SetProperty(ref _creator, value); }
        }

        public AddressModel OwnerAddress
        {
            get { return _ownerAddress; }
            set { SetProperty(ref _ownerAddress, value); }
        }

        public AddressModel CreatorAddress
        {
            get { return _creatorAddress; }
            set { SetProperty(ref _creatorAddress, value); }
        }

        public GetProjectModel Project
        {
            get { return _project; }
            set { SetProperty(ref _project, value); }
        }

        public WorksheetModel Worksheet
        {
            get { return _worksheet; }
            set { SetProperty(ref _worksheet, value); }
        }
    }
}