using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

public class WorksheetWorkflowModel : ViewModelBase
{
    private string _comment;
    private string _displayKey;
    private Guid _worksheetWorkflowId;
    private Guid _idDefaultStep;
    private Guid _idNoModificationsStep;

    public Guid IdNoModificationsStep
    {
        get { return _idNoModificationsStep; }
        set { SetProperty(ref _idNoModificationsStep, value); }
    }

    public Guid IdDefaultStep
    {
        get { return _idDefaultStep; }
        set { SetProperty(ref _idDefaultStep, value); }
    }

    public Guid WorksheetWorkflowId
    {
        get { return _worksheetWorkflowId; }
        set { SetProperty(ref _worksheetWorkflowId, value); }
    }

    public string Comment
    {
        get { return _comment; }
        set { SetProperty(ref _comment, value); }
    }

    public string DisplayKey
    {
        get { return _displayKey; }
        set { SetProperty(ref _displayKey, value); }
    }

    public override Guid? GetModelIdentifier()
    {
        return WorksheetWorkflowId;
    }
}