using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Payment;

public class GetOrder : ViewModelBase
{
    private Guid _idPromisedFeatureContent;

    private bool _isOrderSuccess;

    private string _orderError;
    public Guid OrderId { get; set; }
    public Guid IdAppUser { get; set; }
    public Guid IdPaymentProvider { get; set; }

    public Guid IdPromisedFeatureContent
    {
        get { return _idPromisedFeatureContent; }
        set { SetProperty(ref _idPromisedFeatureContent, value); }
    }

    public bool IsOrderDone { get; set; }
    public string TransactionInfos { get; set; }

    public bool IsOrderSuccess
    {
        get { return _isOrderSuccess; }
        set { SetProperty(ref _isOrderSuccess, value); }
    }

    public string OrderError
    {
        get { return _orderError; }
        set { SetProperty(ref _orderError, value); }
    }


    public override Guid? GetModelIdentifier()
    {
        return OrderId;
    }
}