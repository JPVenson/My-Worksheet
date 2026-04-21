namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Buget;

public class ProjectBudgetViewModel : UpdateProjectBudgetViewModel
{
    private decimal _bugetConsumed;

    private decimal _timeConsumed;

    public decimal TimeConsumed
    {
        get { return _timeConsumed; }
        set { SetProperty(ref _timeConsumed, value); }
    }

    public decimal BugetConsumed
    {
        get { return _bugetConsumed; }
        set { SetProperty(ref _bugetConsumed, value); }
    }
}