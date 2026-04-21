using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.EMail;

[ObjectTracking("MailAccount")]
public class EMailAccountViewModel : ViewModelBase
{
    private string _eMailAddress;
    private Guid _mailAccountId;
    private string _name;
    private int _protocol;
    private string _serverAddress;
    private int _serverPort;
    private string _username;
    private string _password;

    public string Password
    {
        get { return _password; }
        set { SetProperty(ref _password, value); }
    }

    public string Name
    {
        get { return _name; }
        set { SetProperty(ref _name, value); }
    }

    public string EMailAddress
    {
        get { return _eMailAddress; }
        set { SetProperty(ref _eMailAddress, value); }
    }

    public int Protocol
    {
        get { return _protocol; }
        set { SetProperty(ref _protocol, value); }
    }

    public string Username
    {
        get { return _username; }
        set { SetProperty(ref _username, value); }
    }

    public string ServerAddress
    {
        get { return _serverAddress; }
        set { SetProperty(ref _serverAddress, value); }
    }

    public int ServerPort
    {
        get { return _serverPort; }
        set { SetProperty(ref _serverPort, value); }
    }

    public Guid MailAccountId
    {
        get { return _mailAccountId; }
        set { SetProperty(ref _mailAccountId, value); }
    }

    public override Guid? GetModelIdentifier()
    {
        return MailAccountId;
    }
}