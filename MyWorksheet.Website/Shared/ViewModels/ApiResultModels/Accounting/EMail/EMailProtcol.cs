using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.EMail;

public class EMailProtcol : ViewModelBase
{
    private int _id;

    private string _name;

    public int Id
    {
        get { return _id; }
        set { SetProperty(ref _id, value); }
    }

    public string Name
    {
        get { return _name; }
        set { SetProperty(ref _name, value); }
    }

    public override Guid? GetModelIdentifier()
    {
        return null;
    }
}