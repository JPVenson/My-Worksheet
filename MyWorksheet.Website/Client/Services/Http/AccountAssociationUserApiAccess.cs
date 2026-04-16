using System;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Assosiation;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Invites;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Roles;

namespace MyWorksheet.Website.Client.Services.Http;

public class AccountAssociationUserApiAccess : HttpAccessBase
{
    public AccountAssociationUserApiAccess(HttpService httpService) : base(httpService, "AccountUserAssociationApi")
    {
        AdminApi = new AccountAssociationAdminApiAccess(httpService);
    }

    public AccountAssociationAdminApiAccess AdminApi { get; set; }

    public ValueTask<ApiResult<UserToUserRoleViewModel[]>> GetRoles()
    {
        return Get<UserToUserRoleViewModel[]>(BuildApi("GetAssociationRoles"));
    }

    public ValueTask<ApiResult<AssosiationInviteModel[]>> GetInvites()
    {
        return Get<AssosiationInviteModel[]>(BuildApi("GetInvites"));
    }

    public ValueTask<ApiResult<AssosiationInviteRedeemingModel>> PreviewInvite(string inviteCode)
    {
        return Get<AssosiationInviteRedeemingModel>(BuildApi("PreviewInvite", new
        {
            inviteCode
        }));
    }

    public ValueTask<ApiResult> RevokeInvite(Guid inviteId, string reason)
    {
        return Post(BuildApi("RevokeInvite", new
        {
            inviteId,
            reason
        }));
    }

    public ValueTask<ApiResult<AssosiationInviteModel>> CreateUserMappingInvite(InviteCreationViewModel invite)
    {
        return Get<AssosiationInviteModel>(BuildApi("CreateUserMappingInvite", invite));
    }

    public ValueTask<ApiResult<AccountApiGet>> IsAdministratedBy()
    {
        return Get<AccountApiGet>(BuildApi("IsAdministratedBy"));
    }

    public ValueTask<ApiResult> RedeemInviteLink(string externalId)
    {
        return Post(BuildApi("RedeemInviteLink", new
        {
            externalId
        }));
    }

    public ValueTask<ApiResult> RemoveAdministration()
    {
        return Post(BuildApi("RemoveAdministration", new
        {

        }));
    }

    public ValueTask<ApiResult> RemoveUserAssosiations(Guid id)
    {
        return Post(BuildApi("RemoveUserAssosiations", new
        {
            id
        }));
    }

    public ValueTask<ApiResult<UserAssosiationModel[]>> GetUserAssociation()
    {
        return Get<UserAssosiationModel[]>(BuildApi("GetUserAssociation"));
    }

    public ValueTask<ApiResult<PageResultSet<UserToUserAssosiationViewModel>>> GetAssociation(int page, int take, string search = null)
    {
        return Get<PageResultSet<UserToUserAssosiationViewModel>>(BuildApi("GetAssociation", new
        {
            page,
            take,
            search
        }));
    }
}