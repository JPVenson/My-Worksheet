namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Statistics;

public class LabelOversight : ViewModelBase
{
    private string _name;

    public LabelOversight(string name)
    {
        Name = name;
    }

    public string Name
    {
        get { return _name; }
        set { SetProperty(ref _name, value); }
    }
}