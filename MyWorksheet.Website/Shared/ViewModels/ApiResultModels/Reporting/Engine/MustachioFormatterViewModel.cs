namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Engine;

public class MustachioFormatterViewModel : ViewModelBase
{
    private string _description;

    private InputDescriptionViewModel[] _inputDescription;

    private string _inputType;

    private string _name;

    private string _output;

    private string _outputType;

    public string Name
    {
        get { return _name; }
        set { SetProperty(ref _name, value); }
    }

    public string Description
    {
        get { return _description; }
        set { SetProperty(ref _description, value); }
    }

    public InputDescriptionViewModel[] InputDescription
    {
        get { return _inputDescription; }
        set { SetProperty(ref _inputDescription, value); }
    }

    public string InputType
    {
        get { return _inputType; }
        set { SetProperty(ref _inputType, value); }
    }

    public string Output
    {
        get { return _output; }
        set { SetProperty(ref _output, value); }
    }

    public string OutputType
    {
        get { return _outputType; }
        set { SetProperty(ref _outputType, value); }
    }
}