using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

public class OverviewModel : ViewModelBase
{
    private decimal _earned;

    private decimal _honorar;

    private long _hours;

    private Guid _projectId;

    private string _projectName;

    public Guid ProjectId
    {
        get { return _projectId; }
        set { SetProperty(ref _projectId, value); }
    }

    public long Hours
    {
        get { return _hours; }
        set { SetProperty(ref _hours, value); }
    }

    public decimal Earned
    {
        get { return _earned; }
        set { SetProperty(ref _earned, value); }
    }

    public decimal Honorar
    {
        get { return _honorar; }
        set { SetProperty(ref _honorar, value); }
    }

    public string ProjectName
    {
        get { return _projectName; }
        set { SetProperty(ref _projectName, value); }
    }

    public override Guid? GetModelIdentifier()
    {
        return ProjectId;
    }
}