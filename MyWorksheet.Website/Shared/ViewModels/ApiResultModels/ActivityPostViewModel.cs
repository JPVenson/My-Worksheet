using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels;

public class UserActivityPostViewModel : UserActivityViewModel
{
    private Guid _idAppUser;

    public Guid IdAppUser
    {
        get { return _idAppUser; }
        set { SetProperty(ref _idAppUser, value); }
    }
}