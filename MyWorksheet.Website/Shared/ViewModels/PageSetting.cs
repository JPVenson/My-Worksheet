using System.Collections.Generic;

namespace MyWorksheet.Website.Shared.ViewModels;

public class PageSetting : ViewModelBase
{
    public bool IsAdmin
    {
        get { return _isAdmin; }
        set { SetProperty(ref _isAdmin, value); }
    }

    private bool _isAdmin;
    public bool IsLoggedin
    {
        get { return _isLoggedin; }
        set { SetProperty(ref _isLoggedin, value); }
    }

    private bool _isLoggedin;
    public string SessionTimeout
    {
        get { return _sessionTimeout; }
        set { SetProperty(ref _sessionTimeout, value); }
    }

    private string _sessionTimeout;
    public string IssuedDate
    {
        get { return _issuedDate; }
        set { SetProperty(ref _issuedDate, value); }
    }

    private string _issuedDate;
    public bool Persist
    {
        get { return _persist; }
        set { SetProperty(ref _persist, value); }
    }

    private bool _persist;
    public string ServerSessionTimeout
    {
        get { return _serverSessionTimeout; }
        set { SetProperty(ref _serverSessionTimeout, value); }
    }

    private string _serverSessionTimeout;
    public List<string> Roles
    {
        get { return _roles; }
        set { SetProperty(ref _roles, value); }
    }

    private List<string> _roles;
    public string UidToken
    {
        get { return _uidToken; }
        set { SetProperty(ref _uidToken, value); }
    }

    private string _uidToken;
}