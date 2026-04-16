using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Organisation;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Address;

namespace MyWorksheet.Website.Client.Services.Http;

public class OrganizationApiAccess : RestHttpAccessBase<OrganizationSelectionViewModel>
{
    public OrganizationApiAccess(HttpService httpService)
        : base(httpService, "OrganizationApi")
    {
    }

    public Task<IEnumerable<OrganizationViewModel>> Search(string name)
    {
        return Get<PageResultSet<OrganizationViewModel>>(BuildApi("GetForUser", new
        {
            page = 1,
            pageSize = 10,
            includeInactives = false,
            search = name,
        })).AsTask().UnpackList();
    }

    public ValueTask<ApiResult<OrganizationSelectionViewModel[]>> GetAll(bool includeInactive = false, string search = null, Guid? roleId = null)
    {
        return Get<OrganizationSelectionViewModel[]>(BuildApi("GetAllForUser", new
        {
            includeInactive,
            search,
            roleId
        }));
    }

    public ValueTask<ApiResult<OrganisationRoleViewModel[]>> GetRoles()
    {
        return Get<OrganisationRoleViewModel[]>(BuildApi("GetRoles"));
    }

    public ValueTask<ApiResult<OrganizationSelectionViewModel>> Get(Guid id)
    {
        return Get<OrganizationSelectionViewModel>(BuildApi("Get", new
        {
            id
        }));
    }

    public ValueTask<ApiResult<AddressModel>> UpdateAddress(Guid organizationId, AddressModel address)
    {
        return Post<AddressModel, AddressModel>(BuildApi("UpdateAddress", new
        {
            organizationId
        }), address);
    }

    public ValueTask<ApiResult<UpdateOrganizationViewModel>> UpdateOrganization(UpdateOrganizationViewModel model)
    {
        return Post<UpdateOrganizationViewModel, UpdateOrganizationViewModel>(BuildApi("UpdateAddress"), model);
    }

    public ValueTask<ApiResult> AddUserToOrg(Guid organizationId, Guid appUserId, Guid relationId)
    {
        return Post(BuildApi("AddUserToOrg", new
        {
            organizationId,
            appUserId,
            relationId
        }));
    }

    public ValueTask<ApiResult> RemoveUserToOrg(Guid organizationId, Guid appUserId, Guid relationId)
    {
        return Post(BuildApi("RemoveUserToOrg", new
        {
            organizationId,
            appUserId,
            relationId
        }));
    }

    public ValueTask<ApiResult<UpdateOrganizationViewModel>> Create(OrganizationGroupViewModel model)
    {
        return Post<OrganizationGroupViewModel, UpdateOrganizationViewModel>(BuildApi("Create"), model);
    }

    public ValueTask<ApiResult<UpdateOrganizationViewModel>> Update(OrganizationGroupViewModel model, Guid id)
    {
        return Post<OrganizationGroupViewModel, UpdateOrganizationViewModel>(BuildApi("Update", new
        {
            id
        }), model);
    }

    public ValueTask<ApiResult> Delete(Guid id)
    {
        return Post(BuildApi("Delete", new
        {
            id
        }));
    }

    public ValueTask<ApiResult<PageResultSet<OrganizationMapViewModel>>> GetUsersInOrg(Guid organizationId, int page, int pageSize)
    {
        return Get<PageResultSet<OrganizationMapViewModel>>(BuildApi("GetUsersInOrg", new
        {
            organizationId,
            page,
            pageSize
        }));
    }

    public ValueTask<ApiResult<PageResultSet<AccountApiUserGetInfo>>> SearchUsersInOrg(Guid organizationId, int page, int pageSize, string username = null)
    {
        return Get<PageResultSet<AccountApiUserGetInfo>>(BuildApi("SearchUsersInOrg", new
        {
            organizationId,
            page,
            pageSize,
            username
        }));
    }

    public ValueTask<ApiResult<OrganizationSelectionViewModel[]>> AdminGetAllForUser(Guid userId, bool includeInactive = false, string search = null)
    {
        return Get<OrganizationSelectionViewModel[]>(BuildApi("Admin/GetAllForUser", new
        {
            userId,
            includeInactive,
            search
        }));
    }

    public ValueTask<ApiResult<PageResultSet<OrganizationSelectionViewModel>>> AdminGetForUser(Guid userId, int page, int pageSize, bool includeInactives = false, string search = null)
    {
        return Get<PageResultSet<OrganizationSelectionViewModel>>(BuildApi("Admin/GetForUser", new
        {
            userId,
            page,
            pageSize,
            includeInactives,
            search
        }));
    }
}