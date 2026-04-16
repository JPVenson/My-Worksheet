using System.Collections.Generic;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet
{
    public class PostProjectApiModel : ViewModelBase
    {
        private IList<ApiEntityState<ProjectItemRateViewModel>> _chargeRates;

        private PostProjectModel _project;

        public PostProjectModel Project
        {
            get { return _project; }
            set { SetProperty(ref _project, value); }
        }

        public IList<ApiEntityState<ProjectItemRateViewModel>> ChargeRates
        {
            get { return _chargeRates; }
            set { SetProperty(ref _chargeRates, value); }
        }
    }
}