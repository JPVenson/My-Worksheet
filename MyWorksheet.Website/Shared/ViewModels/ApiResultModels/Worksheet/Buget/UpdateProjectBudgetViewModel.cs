using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Buget;

public class UpdateProjectBudgetViewModel : CreateProjectBudgetViewModel
{
    private Guid _projectBudgetId;

    private byte[] _rowVersion;

    public Guid ProjectBudgetId
    {
        get { return _projectBudgetId; }
        set { SetProperty(ref _projectBudgetId, value); }
    }

    public byte[] RowVersion
    {
        get { return _rowVersion; }
        set { SetProperty(ref _rowVersion, value); }
    }

    public override Guid? GetModelIdentifier()
    {
        return ProjectBudgetId;
    }
}