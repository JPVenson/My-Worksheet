using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;

public class AccountApiGet : ViewModelBase
{
    private string _contactName;

    private string _email;

    private Guid? _idAddress;

    private string _username;

    public string Username
    {
        get { return _username; }
        set { SetProperty(ref _username, value); }
    }

    public string Email
    {
        get { return _email; }
        set { SetProperty(ref _email, value); }
    }

    public string ContactName
    {
        get { return _contactName; }
        set { SetProperty(ref _contactName, value); }
    }

    public Guid? IdAddress
    {
        get { return _idAddress; }
        set { SetProperty(ref _idAddress, value); }
    }
}