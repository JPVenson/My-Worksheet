using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Payment
{
    public class PaymentInfoModel : ViewModelBase
    {
        private string _paymentDisclaimer;
        private Guid _paymentInfoId;
        private string _paymentType;
        private int? _var;

        public int? PaymentTarget
        {
            get { return _var; }
            set { SetProperty(ref _var, value); }
        }

        public Guid PaymentInfoId
        {
            get { return _paymentInfoId; }
            set { SetProperty(ref _paymentInfoId, value); }
        }

        public string PaymentType
        {
            get { return _paymentType; }
            set { SetProperty(ref _paymentType, value); }
        }

        public string PaymentDisclaimer
        {
            get { return _paymentDisclaimer; }
            set { SetProperty(ref _paymentDisclaimer, value); }
        }

        public override Guid? GetModelIdentifier()
        {
            return PaymentInfoId;
        }
    }
}