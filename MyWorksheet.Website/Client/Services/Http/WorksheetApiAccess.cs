using System;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

namespace MyWorksheet.Website.Client.Services.Http;

public class WorksheetApiAccess : RestHttpAccessBase<WorksheetModel, WorksheetModel>
{
    public WorksheetApiAccess(HttpService httpService)
        : base(httpService, "WorksheetApi")
    {
    }

    public ValueTask<ApiResult<WorksheetModel[]>> GetByProject(Guid projectId, bool? showHidden = null)
    {
        return Get<WorksheetModel[]>(BuildApi("Worksheets", new
        {
            projectId,
            showHidden
        }));
    }
}