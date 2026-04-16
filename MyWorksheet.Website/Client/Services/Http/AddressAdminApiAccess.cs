using System;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Address;

namespace MyWorksheet.Website.Client.Services.Http;

public class AddressAdminApiAccess : HttpAccessBase
{
    public AddressAdminApiAccess(HttpService httpService) : base(httpService, "AddressApi/Admin")
    {
    }

    public ValueTask<ApiResult<AddressModel[]>> UserAddress(Guid id)
    {
        return Get<AddressModel[]>(BuildApi("UserAddress", new
        {
            id = id
        }));
    }

    public ValueTask<ApiResult<AddressModel>> Update(UpdateAddressModel addressModel, Guid id, Guid userId)
    {
        return Post<UpdateAddressModel, AddressModel>(BuildApi("Update", new
        {
            id = id,
            userId = userId
        }), addressModel);
    }
}