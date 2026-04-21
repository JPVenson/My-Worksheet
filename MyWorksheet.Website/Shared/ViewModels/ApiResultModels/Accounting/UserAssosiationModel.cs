using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;

public class UserAssosiationModel : ViewModelBase
{
    private Guid _userAssoisiatedUserMapId;

    public Guid UserAssoisiatedUserMapId
    {
        get { return _userAssoisiatedUserMapId; }
        set { SetProperty(ref _userAssoisiatedUserMapId, value); }
    }

    public Guid IdParentUser { get; set; }
    public Guid IdChildUser { get; set; }
    public Guid IdUserRelation { get; set; }

    public override Guid? GetModelIdentifier()
    {
        return UserAssoisiatedUserMapId;
    }
}