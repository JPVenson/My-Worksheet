using System;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Address;

namespace MyWorksheet.Website.Client.Services.Http;

public class AddressApiAccess : HttpAccessBase
{
    public AddressApiAccess(HttpService httpService) : base(httpService, "AddressApi")
    {
        AdminApi = new AddressAdminApiAccess(httpService);
    }

    public AddressAdminApiAccess AdminApi { get; set; }

    public ValueTask<ApiResult<AddressModel>> MyAddress()
    {
        return Get<AddressModel>(BuildApi("MyAddress"));
    }

    public ValueTask<ApiResult<AddressModel>> UserAddress(Guid id)
    {
        return Get<AddressModel>(BuildApi("UserAddress", new
        {
            id = id
        }));
    }

    public ValueTask<ApiResult<AddressModel>> OrganizationAddress(Guid id)
    {
        return Get<AddressModel>(BuildApi("OrganizationAddress", new
        {
            id = id
        }));
    }

    public ValueTask<ApiResult<AddressModel>> Update(UpdateAddressModel addressModel, Guid id)
    {
        return Post<UpdateAddressModel, AddressModel>(BuildApi("Update", new
        {
            id = id
        }), addressModel);
    }
}