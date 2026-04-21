using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

public class WorksheetStatusLookupModel : ViewModelBase
{
    private string _action;

    private string _description;

    private Guid? _idAppUser;

    private bool _isPersitent;

    public string Description
    {
        get { return _description; }
        set { SetProperty(ref _description, value); }
    }

    public string Action
    {
        get { return _action; }
        set { SetProperty(ref _action, value); }
    }

    public bool IsPersitent
    {
        get { return _isPersitent; }
        set { SetProperty(ref _isPersitent, value); }
    }

    public Guid? IdAppUser
    {
        get { return _idAppUser; }
        set { SetProperty(ref _idAppUser, value); }
    }
}