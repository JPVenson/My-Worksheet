using System;
using System.ComponentModel.DataAnnotations;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Address;

public class UpdateAddressModel : ViewModelBase
{
    private Guid _addressId;

    private string _city;

    private string _companyName;

    private string _email;

    private string _phone;

    private string _street;

    private string _streetNo;

    private string _zipCode;

    [Required]
    public string CompanyName
    {
        get { return _companyName; }
        set { SetProperty(ref _companyName, value); }
    }

    [Required]
    public Guid AddressId
    {
        get { return _addressId; }
        set { SetProperty(ref _addressId, value); }
    }

    [Required]
    public string Street
    {
        get { return _street; }
        set { SetProperty(ref _street, value); }
    }

    [Required]
    public string StreetNo
    {
        get { return _streetNo; }
        set { SetProperty(ref _streetNo, value); }
    }

    [Required]
    public string ZipCode
    {
        get { return _zipCode; }
        set { SetProperty(ref _zipCode, value); }
    }

    [Required]
    public string City
    {
        get { return _city; }
        set { SetProperty(ref _city, value); }
    }

    [Required]
    public string Phone
    {
        get { return _phone; }
        set { SetProperty(ref _phone, value); }
    }

    [Required]
    public string Email
    {
        get { return _email; }
        set { SetProperty(ref _email, value); }
    }

    public override Guid? GetModelIdentifier()
    {
        return AddressId;
    }
}