namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Payment;

public class CurrencyLookup : ViewModelBase
{
    private string _isoCode;

    private string _sign;

    public string IsoCode
    {
        get { return _isoCode; }
        set { SetProperty(ref _isoCode, value); }
    }

    public string Sign
    {
        get { return _sign; }
        set { SetProperty(ref _sign, value); }
    }
}