using System;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;

namespace MyWorksheet.Website.Client.Services.Http;

public class AdminAccountApiAccess : HttpAccessBase
{
    public AdminAccountApiAccess(HttpService httpService)
        : base(httpService, "AccountApi/Admin")
    {
    }

    public ValueTask<ApiResult<Maintainance>> UpdateMaintance(MaintainceModeUpdateViewModel data)
    {
        return Post<MaintainceModeUpdateViewModel, Maintainance>(BuildApi("UpdateMaintance"), data);
    }

    public ValueTask<ApiResult<Maintainance>> GetMaintaince()
    {
        return Get<Maintainance>(BuildApi("Maintaince"));
    }

    public ValueTask<ApiResult<PageResultSet<AccountApiAdminGet>>> SearchUsers(int page, int take, bool includeTestUsers = false, string search = null)
    {
        return Get<PageResultSet<AccountApiAdminGet>>(BuildApi("GetAll", new
        {
            page,
            take,
            includeTestUsers,
            search
        }));
    }

    public ValueTask<ApiResult<AccountApiAdminGet>> CreateUser(AccountApiUserCreate data)
    {
        return Post<AccountApiUserCreate, AccountApiAdminGet>(BuildApi("Create"), data);
    }

    public ValueTask<ApiResult> ChangePassword(AccountApiUserChangeUserPassword data)
    {
        return Post(BuildApi("ChangePassword"), data);
    }

    public ValueTask<ApiResult> Delete(Guid userId)
    {
        return Post(BuildApi("Delete", new
        {
            id = userId
        }));
    }

    public ValueTask<ApiResult> DeleteAccount(Guid userId)
    {
        return Post(BuildApi("DeleteAccount", new
        {
            id = userId
        }));
    }

    public ValueTask<ApiResult<AccountApiAdminGet>> Get(Guid userId)
    {
        return Get<AccountApiAdminGet>(BuildApi("User", new
        {
            userId
        }));
    }

    public ValueTask<ApiResult<AccountApiAdminGet>> UpdateAccount(AccountApiAdminGet userAccount)
    {
        return Post<AccountApiAdminGet, AccountApiAdminGet>(BuildApi("Update"), userAccount);
    }

    public ValueTask<ApiResult<UserActionAdminGet[]>> GetUserActions(Guid userId)
    {
        return Get<UserActionAdminGet[]>(BuildApi("GetUserActions", new
        {
            userId
        }));
    }

    public ValueTask<ApiResult<UserQuotaViewModel[]>> GetCounterInfos(Guid userId)
    {
        return Get<UserQuotaViewModel[]>(BuildApi("GetCounterInfos", new
        {
            userId
        }));
    }

    public ValueTask<ApiResult> UpdateCounterInfos(UpdateUserQuotasViewModel model)
    {
        return Post<UpdateUserQuotasViewModel>(BuildApi("UpdateCounterInfos"), model);
    }
}