using System;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;

public class UserActionAdminGet : ViewModelBase
{
    private string _actionIP;

    private DateTime? _date;

    private Guid _idUser;

    private string _pcName;

    private Guid _userActionId;

    private byte _userActionType;

    private string _userAgent;

    public Guid UserActionId
    {
        get { return _userActionId; }
        set { SetProperty(ref _userActionId, value); }
    }

    public byte UserActionType
    {
        get { return _userActionType; }
        set { SetProperty(ref _userActionType, value); }
    }

    public DateTime? Date
    {
        get { return _date; }
        set { SetProperty(ref _date, value); }
    }

    public string ActionIP
    {
        get { return _actionIP; }
        set { SetProperty(ref _actionIP, value); }
    }

    public string UserAgent
    {
        get { return _userAgent; }
        set { SetProperty(ref _userAgent, value); }
    }

    public string PcName
    {
        get { return _pcName; }
        set { SetProperty(ref _pcName, value); }
    }

    public Guid IdUser
    {
        get { return _idUser; }
        set { SetProperty(ref _idUser, value); }
    }

    public override Guid? GetModelIdentifier()
    {
        return UserActionId;
    }
}