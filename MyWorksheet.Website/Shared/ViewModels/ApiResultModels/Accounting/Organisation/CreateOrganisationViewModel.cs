using System;
using System.ComponentModel.DataAnnotations;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Address;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Organisation;

public class OrganizationGroupViewModel
{
    public OrganizationViewModel OrganizationViewModel { get; set; }
    public AddressModel Address { get; set; }
    public ApiEntityState<UpdateOrganizationMapViewModel>[] Users { get; set; }
}

public class CreateOrganisationViewModel : ViewModelBase
{
    private AddressModel _address;

    private Guid? _idParentOrganisation;

    private string _name;

    private string _sharedId;

    [Required]
    public AddressModel Address
    {
        get { return _address; }
        set { SetProperty(ref _address, value); }
    }

    [Required]
    public string Name
    {
        get { return _name; }
        set { SetProperty(ref _name, value); }
    }

    [Required]
    public string SharedId
    {
        get { return _sharedId; }
        set { SetProperty(ref _sharedId, value); }
    }

    public Guid? IdParentOrganisation
    {
        get { return _idParentOrganisation; }
        set { SetProperty(ref _idParentOrganisation, value); }
    }
}