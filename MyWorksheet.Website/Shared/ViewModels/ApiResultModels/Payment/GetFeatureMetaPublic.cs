using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Payment
{
    public class GetFeatureMetaPublic : ViewModelBase
    {
        private string _displayKey;

        private bool _inclusiveFeature;

        private bool _isActive;

        private Guid _promisedFeatureId;

        private bool _reoccuringFeature;

        public Guid PromisedFeatureId
        {
            get { return _promisedFeatureId; }
            set { SetProperty(ref _promisedFeatureId, value); }
        }

        public string DisplayKey
        {
            get { return _displayKey; }
            set { SetProperty(ref _displayKey, value); }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set { SetProperty(ref _isActive, value); }
        }

        public bool ReoccuringFeature
        {
            get { return _reoccuringFeature; }
            set { SetProperty(ref _reoccuringFeature, value); }
        }

        public bool InclusiveFeature
        {
            get { return _inclusiveFeature; }
            set { SetProperty(ref _inclusiveFeature, value); }
        }

        public override Guid? GetModelIdentifier()
        {
            return PromisedFeatureId;
        }
    }
}