using System;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Organisation;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.UserWorkload;

namespace MyWorksheet.Website.Client.Services.Http;

public class UserWorkloadApiAccess : RestHttpAccessBase<GetUserWorkloadViewModel>
{
    public UserWorkloadApiAccess(HttpService httpService)
        : base(httpService, "UserWorkloadApi")
    {
    }

    public ValueTask<ApiResult<GetUserWorkloadViewModel>> GetWorkloadForProject(Guid projectId)
    {
        return Get<GetUserWorkloadViewModel>(BuildApi("GetWorkloadForProject",
            new
            {
                projectId
            }));
    }

    public ValueTask<ApiResult<GetUserWorkloadViewModel[]>> GetWorkloadsForProjects(Guid[] projectIds)
    {
        return Get<GetUserWorkloadViewModel[]>(BuildApi("GetWorkloadsForProjects",
            new
            {
                projectIds
            }));
    }

    public ValueTask<ApiResult<GetUserWorkloadViewModel>> GetDefaultWorkload()
    {
        return Get<GetUserWorkloadViewModel>(BuildApi("GetUsersWorkload"));
    }

    public ValueTask<ApiResult<GetUserWorkloadViewModel>> Update(UpdateUserWorkloadViewModel model)
    {
        return Post<UpdateUserWorkloadViewModel, GetUserWorkloadViewModel>(BuildApi("Update"), model);
    }

    public ValueTask<ApiResult<GetUserWorkloadViewModel>> Create(CreateUserWorkloadViewModel model)
    {
        return Post<CreateUserWorkloadViewModel, GetUserWorkloadViewModel>(BuildApi("Create"), model);
    }

    public ValueTask<ApiResult> Delete(Guid userWorkloadId)
    {
        return Post(BuildApi("Delete", new { userWorkloadId }));
    }
}