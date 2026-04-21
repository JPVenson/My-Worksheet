namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Engine;

public class ReportingParamterValue : ViewModelBase
{
    private string _name;

    private object _value;

    public ReportingParamterValue()
    {
    }

    public ReportingParamterValue(object value)
    {
        Name = value.ToString();
        Value = value;
    }

    public ReportingParamterValue(string name, object value)
    {
        Name = name;
        Value = value;
    }

    public string Name
    {
        get { return _name; }
        set { SetProperty(ref _name, value); }
    }

    public object Value
    {
        get { return _value; }
        set { SetProperty(ref _value, value); }
    }
}