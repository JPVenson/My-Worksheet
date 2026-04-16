using System;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

namespace MyWorksheet.Website.Client.Services.Http;

public class ProjectApiAccess : RestHttpAccessBase<GetProjectModel, PostProjectApiModel>
{
    public ProjectApiAccess(HttpService httpService)
        : base(httpService, "ProjectApi")
    {
    }

    public ValueTask<ApiResult<PageResultSet<GetProjectModel>>>
        SearchProjects(int page, int pageSize, string search = null, bool showHidden = false)
    {
        return base.Get<PageResultSet<GetProjectModel>>(BuildApi("Projects",
            new
            {
                page,
                pageSize,
                search,
                showHidden
            }
        ));
    }

    public ValueTask<ApiResult<GetProjectModel[]>> GetProjectsByOrganisation(Guid organisationId)
    {
        return Get<GetProjectModel[]>(BuildApi("GetByOrganisation", new { organisationId }));
    }
}