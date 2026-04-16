using System;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Buget;

namespace MyWorksheet.Website.Client.Services.Http;

public class ProjectBudgetApiAccess : HttpAccessBase
{
    public ProjectBudgetApiAccess(HttpService httpService)
        : base(httpService, "ProjectBudgetApi")
    {
    }

    public ValueTask<ApiResult<ProjectBudgetViewModel>> GetUsersBudget(Guid projectId)
    {
        return Get<ProjectBudgetViewModel>(BuildApi("GetUsersBudget", new { projectId }));
    }

    public ValueTask<ApiResult<ProjectBudgetViewModel[]>> GetProjectBudgets(Guid projectId)
    {
        return Get<ProjectBudgetViewModel[]>(BuildApi("GetProjectBudgets", new { projectId }));
    }

    public ValueTask<ApiResult<ProjectBudgetViewModel>> CreateBudget(CreateProjectBudgetViewModel budget)
    {
        return Post<CreateProjectBudgetViewModel, ProjectBudgetViewModel>(BuildApi("CreateBudget"), budget);
    }

    public ValueTask<ApiResult<ProjectBudgetViewModel>> UpdateBudget(UpdateProjectBudgetViewModel budget)
    {
        return Post<UpdateProjectBudgetViewModel, ProjectBudgetViewModel>(BuildApi("UpdateBudget"), budget);
    }

    public ValueTask<ApiResult> DeleteBudget(Guid budgetId)
    {
        return Post(BuildApi("DeleteBudget", new { budgetId }));
    }
}
