using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Payment
{
    public class FeatureOverviewAggregatedFeatures : ViewModelBase
    {
        private int? _ammount;

        private Guid _idPromisedFeatureContent;

        public Guid IdPromisedFeatureContent
        {
            get { return _idPromisedFeatureContent; }
            set { SetProperty(ref _idPromisedFeatureContent, value); }
        }

        public int? Ammount
        {
            get { return _ammount; }
            set { SetProperty(ref _ammount, value); }
        }
    }
}