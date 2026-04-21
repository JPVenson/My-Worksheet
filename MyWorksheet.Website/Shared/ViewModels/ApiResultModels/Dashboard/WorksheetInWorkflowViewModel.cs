using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Dashboard;

public class WorksheetInWorkflowViewModel : ViewModelBase
{
    private decimal _workedTimes;

    private WorksheetModel _worksheet;

    public decimal WorkedTimes
    {
        get { return _workedTimes; }
        set { SetProperty(ref _workedTimes, value); }
    }

    public WorksheetModel Worksheet
    {
        get { return _worksheet; }
        set { SetProperty(ref _worksheet, value); }
    }
}