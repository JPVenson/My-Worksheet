namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

public class DashboardWorksheetModel : WorksheetModel
{
    private bool _hasDaysOpen;

    private string _projectName;

    public string ProjectName
    {
        get { return _projectName; }
        set { SetProperty(ref _projectName, value); }
    }

    public bool HasDaysOpen
    {
        get { return _hasDaysOpen; }
        set { SetProperty(ref _hasDaysOpen, value); }
    }
}