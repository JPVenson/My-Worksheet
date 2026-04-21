namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;

[ObjectTracking("UserQuota")]
public class UserQuotaViewModel : ViewModelBase
{
    private long _maxValue;

    private string _name;

    private int _type;

    private long _value;

    public string Name
    {
        get { return _name; }
        set { SetProperty(ref _name, value); }
    }

    public int Type
    {
        get { return _type; }
        set { SetProperty(ref _type, value); }
    }

    public long MaxValue
    {
        get { return _maxValue; }
        set { SetProperty(ref _maxValue, value); }
    }

    public long Value
    {
        get { return _value; }
        set { SetProperty(ref _value, value); }
    }
}

//public class UserCounterViewModel
//{
//	public Guid UserCounterId { get; set; }
//	public Guid UserId { get; set; }

//	public int ProjectCounter { get; set; }
//	public int WorksheetCounter { get; set; }
//	public long ReportLocalSizeCounter { get; set; }
//	public int WorksheetItemCounter { get; set; }
//	public int WebhookCounter { get; set; }

//	public int ProjectLengthCounter { get; set; }
//	public int WorksheetLengthCounter { get; set; }
//	public int WebhookLengthCounter { get; set; }
//	public long ReportLocalSizeLengthCounter { get; set; }
//	public int ProjectLength { get; set; }
//	public int WorksheetLength { get; set; }
//}