namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.NumberRange;

public class NumberRangeCodeMap : ViewModelBase
{
    private string _code;

    private string _descriptionText;

    public string Code
    {
        get { return _code; }
        set { SetProperty(ref _code, value); }
    }

    public string DescriptionText
    {
        get { return _descriptionText; }
        set { SetProperty(ref _descriptionText, value); }
    }
}