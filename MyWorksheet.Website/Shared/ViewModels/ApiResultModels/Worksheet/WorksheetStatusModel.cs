using System;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

public class WorksheetStatusModel : ViewModelBase
{
    private DateTime _dateOfAction;

    private Guid _idChangeUser;

    private Guid _idPostState;

    private Guid? _idPreState;

    private Guid _idWorksheet;

    private string _reason;

    private Guid _worksheetStatusHistoryId;

    public Guid WorksheetStatusHistoryId
    {
        get { return _worksheetStatusHistoryId; }
        set { SetProperty(ref _worksheetStatusHistoryId, value); }
    }

    public DateTime DateOfAction
    {
        get { return _dateOfAction; }
        set { SetProperty(ref _dateOfAction, value); }
    }

    public string Reason
    {
        get { return _reason; }
        set { SetProperty(ref _reason, value); }
    }

    public Guid IdWorksheet
    {
        get { return _idWorksheet; }
        set { SetProperty(ref _idWorksheet, value); }
    }

    public Guid? IdPreState
    {
        get { return _idPreState; }
        set { SetProperty(ref _idPreState, value); }
    }

    public Guid IdPostState
    {
        get { return _idPostState; }
        set { SetProperty(ref _idPostState, value); }
    }

    public Guid IdChangeUser
    {
        get { return _idChangeUser; }
        set { SetProperty(ref _idChangeUser, value); }
    }

    public override Guid? GetModelIdentifier()
    {
        return WorksheetStatusHistoryId;
    }
}