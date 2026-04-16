using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Administration;

namespace MyWorksheet.Website.Client.Services.Http;

public class AccountApiAccess : HttpAccessBase
{
    public AccountApiAccess(HttpService httpService)
        : base(httpService, "AccountApi")
    {
        AdminApi = new AdminAccountApiAccess(httpService);
    }

    public AdminAccountApiAccess AdminApi { get; set; }

    public ValueTask<ApiResult<string>> RequestPasswordChange(AccountPasswordRequestPost data)
    {
        return Post<AccountPasswordRequestPost, string>(BuildApi("ResetPasswordRequest"), data);
    }

    public ValueTask<ApiResult<ClientSettingsModel>> GetPageSettings()
    {
        return Get<ClientSettingsModel>(BuildApi("GetPageSettings"));
    }

    public ValueTask<ApiResult> ChangePassword(AccountApiUserChangePassword data)
    {
        return Post<AccountApiUserChangePassword>(BuildApi("ChangePassword"), data);
    }

    public ValueTask<ApiResult> ResetPassword(AccountApiUserResetPassword data)
    {
        return Post<AccountApiUserResetPassword>(BuildApi("ResetPassword"), data);
    }

    public ValueTask<ApiResult> UpdateUserData(AccountApiUserPost data)
    {
        return Post(BuildApi("UpdateCurrentUserData"), data);
    }

    public ValueTask<ApiResult<PageResultSet<AccountApiUserGetInfo>>> GetAssosiatedUsers(int page, int take, string search = null)
    {
        return Get<PageResultSet<AccountApiUserGetInfo>>(BuildApi("GetAll", new
        {
            page,
            take,
            search
        }));
    }

    public ValueTask<ApiResult<UserQuotaViewModel[]>> UserQuota(int? quotaType = null)
    {
        return Get<UserQuotaViewModel[]>(BuildApi("GetCounterInfos", new
        {
            quotaType
        }));
    }
}