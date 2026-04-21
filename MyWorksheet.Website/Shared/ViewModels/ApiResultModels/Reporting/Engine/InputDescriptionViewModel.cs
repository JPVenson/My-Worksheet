namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Engine;

public class InputDescriptionViewModel : ViewModelBase
{
    private string _output;
    public string Description { get; set; }
    public string Example { get; set; }
    public string OutputType { get; set; }

    public string Output
    {
        get { return _output; }
        set { SetProperty(ref _output, value); }
    }
}