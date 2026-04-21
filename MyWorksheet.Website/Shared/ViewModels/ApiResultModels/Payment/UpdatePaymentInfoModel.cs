using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Payment;

public class UpdatePaymentInfoModel : ViewModelBase
{
    private ApiEntityState<PaymentInfoContentModel>[] _contentModels;

    private PaymentInfoModel _paymentInfoModel;

    public PaymentInfoModel PaymentInfoModel
    {
        get { return _paymentInfoModel; }
        set { SetProperty(ref _paymentInfoModel, value); }
    }

    public ApiEntityState<PaymentInfoContentModel>[] ContentModels
    {
        get { return _contentModels; }
        set { SetProperty(ref _contentModels, value); }
    }
}