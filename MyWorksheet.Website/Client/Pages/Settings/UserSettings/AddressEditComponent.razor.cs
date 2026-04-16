using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Address;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Settings.UserSettings;

public partial class AddressEditComponent
{
    public AddressEditComponent()
    {

    }


    private EntityState<AddressModel> _address;

    [Parameter]
    public EntityState<AddressModel> Address
    {
        get { return _address; }
        set { SetProperty(ref _address, value, AddressChanged); }
    }

    [Parameter]
    public EventCallback<EntityState<AddressModel>> AddressChanged { get; set; }

    [Parameter]
    public bool AllowChangeOfCountry { get; set; }
}