using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Payment
{
    public class GetFeature : ViewModelBase
    {
        private string _descriptionLong;

        private string _descriptionShort;

        private string _displayKey;

        private Guid _idPromisedFeature;

        private bool _isActive;

        private decimal _price;

        private Guid _promisedFeatureContentId;

        public Guid PromisedFeatureContentId
        {
            get { return _promisedFeatureContentId; }
            set { SetProperty(ref _promisedFeatureContentId, value); }
        }

        public string DisplayKey
        {
            get { return _displayKey; }
            set { SetProperty(ref _displayKey, value); }
        }

        public string DescriptionShort
        {
            get { return _descriptionShort; }
            set { SetProperty(ref _descriptionShort, value); }
        }

        public string DescriptionLong
        {
            get { return _descriptionLong; }
            set { SetProperty(ref _descriptionLong, value); }
        }

        public decimal Price
        {
            get { return _price; }
            set { SetProperty(ref _price, value); }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set { SetProperty(ref _isActive, value); }
        }

        public Guid IdPromisedFeature
        {
            get { return _idPromisedFeature; }
            set { SetProperty(ref _idPromisedFeature, value); }
        }

        public override Guid? GetModelIdentifier()
        {
            return PromisedFeatureContentId;
        }
    }
}