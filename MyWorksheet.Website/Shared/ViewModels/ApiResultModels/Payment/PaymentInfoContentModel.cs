using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Payment
{
    public class PaymentInfoContentModel : ViewModelBase
    {
        private string _fieldName;

        private string _fieldValue;

        private Guid _paymentInfoContentId;

        public Guid PaymentInfoContentId
        {
            get { return _paymentInfoContentId; }
            set { SetProperty(ref _paymentInfoContentId, value); }
        }

        public string FieldName
        {
            get { return _fieldName; }
            set { SetProperty(ref _fieldName, value); }
        }

        public string FieldValue
        {
            get { return _fieldValue; }
            set { SetProperty(ref _fieldValue, value); }
        }

        public override Guid? GetModelIdentifier()
        {
            return PaymentInfoContentId;
        }
    }
}