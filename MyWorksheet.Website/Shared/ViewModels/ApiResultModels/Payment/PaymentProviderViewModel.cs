using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Payment
{
    public class PaymentProviderViewModel : ViewModelBase
    {
        private string _iconUrl;

        private string _name;

        private Guid _paymentProviderId;

        private int _region;

        public Guid PaymentProviderId
        {
            get { return _paymentProviderId; }
            set { SetProperty(ref _paymentProviderId, value); }
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public int Region
        {
            get { return _region; }
            set { SetProperty(ref _region, value); }
        }

        public string IconUrl
        {
            get { return _iconUrl; }
            set { SetProperty(ref _iconUrl, value); }
        }

        public override Guid? GetModelIdentifier()
        {
            return PaymentProviderId;
        }
    }
}