using System;
using System.ComponentModel.DataAnnotations;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Organisation;

[ObjectTracking("Organisation")]
public class UpdateOrganizationViewModel : ViewModelBase
{
    private Guid? _idParentOrganisation;

    private bool _isActive;

    private Guid _organisationId;

    private string _sharedId;

    [IdentityProperty]
    public Guid OrganisationId
    {
        get { return _organisationId; }
        set { SetProperty(ref _organisationId, value); }
    }

    public Guid? IdParentOrganisation
    {
        get { return _idParentOrganisation; }
        set { SetProperty(ref _idParentOrganisation, value); }
    }

    [Required]
    public string SharedId
    {
        get { return _sharedId; }
        set { SetProperty(ref _sharedId, value); }
    }

    public bool IsActive
    {
        get { return _isActive; }
        set { SetProperty(ref _isActive, value); }
    }

    public override Guid? GetModelIdentifier()
    {
        return OrganisationId;
    }
}