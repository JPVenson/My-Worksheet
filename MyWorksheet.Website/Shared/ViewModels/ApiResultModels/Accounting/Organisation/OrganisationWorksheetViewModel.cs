using System;
using System.Collections.Generic;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Organisation;

public class OrganisationWorksheetViewModel : ViewModelBase
{
    private Guid _idAppUser;

    private bool _isActive;

    private Guid _organisationId;

    private ICollection<WorksheetModel> _worksheets;

    public Guid OrganisationId
    {
        get { return _organisationId; }
        set { SetProperty(ref _organisationId, value); }
    }

    public bool IsActive
    {
        get { return _isActive; }
        set { SetProperty(ref _isActive, value); }
    }

    public Guid IdAppUser
    {
        get { return _idAppUser; }
        set { SetProperty(ref _idAppUser, value); }
    }

    public ICollection<WorksheetModel> Worksheets
    {
        get { return _worksheets; }
        set { SetProperty(ref _worksheets, value); }
    }

    public override Guid? GetModelIdentifier()
    {
        return OrganisationId;
    }
}