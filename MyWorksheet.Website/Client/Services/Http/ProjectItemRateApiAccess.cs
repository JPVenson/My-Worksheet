using System;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

namespace MyWorksheet.Website.Client.Services.Http;

public class ProjectItemRateApiAccess : RestHttpAccessBase<ProjectItemRateViewModel>
{
    public ProjectItemRateApiAccess(HttpService httpService)
        : base(httpService, "ProjectItemRateApi")
    {
    }

    public ValueTask<ApiResult<ProjectItemRateViewModel[]>> GetRatesForProject(Guid projectId)
    {
        return Get<ProjectItemRateViewModel[]>(BuildApi("GetProjectRates",
            new
            {
                projectId
            }));
    }

    public ValueTask<ApiResult<ProjectItemRateViewModel[]>> GetRatesForProject(Guid[] projectIds)
    {
        return Get<ProjectItemRateViewModel[]>(BuildApi("GetProjectsRates",
            new
            {
                projectIds
            }));
    }

    public ValueTask<ApiResult<ProjectChargeRateModel[]>> GetChargeRates()
    {
        return Get<ProjectChargeRateModel[]>(BuildApi("GetProjectChargeRates"));
    }

    public ValueTask<ApiResult<ProjectItemRateViewModel>> Create(ProjectItemRateViewModel model)
    {
        return Post<ProjectItemRateViewModel, ProjectItemRateViewModel>(BuildApi("Create"), model);
    }

    public ValueTask<ApiResult<ProjectItemRateViewModel>> Update(ProjectItemRateViewModel model)
    {
        return Post<ProjectItemRateViewModel, ProjectItemRateViewModel>(BuildApi("Update"), model);
    }

    public ValueTask<ApiResult> Delete(Guid id)
    {
        return Post(BuildApi("Delete", new
        {
            id
        }));
    }
}