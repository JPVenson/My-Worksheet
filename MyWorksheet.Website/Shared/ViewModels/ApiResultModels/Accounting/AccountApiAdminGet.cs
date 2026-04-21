namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;

public class AccountApiAdminGet : AccountApiAdminPost
{
    private System.Guid _userID;

    public System.Guid UserID
    {
        get { return _userID; }
        set { SetProperty(ref _userID, value); }
    }
}