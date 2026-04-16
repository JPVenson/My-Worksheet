using System;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Workflow;

namespace MyWorksheet.Website.Client.Services.Http;

public class WorksheetHistoryApiAccess : HttpAccessBase
{
    public WorksheetHistoryApiAccess(HttpService httpService)
        : base(httpService, "WorksheetHistoryApi")
    {
    }

    public ValueTask<ApiResult<WorksheetWorkflowStatusLookupViewModel[]>> GetLookups()
    {
        return Get<WorksheetWorkflowStatusLookupViewModel[]>(BuildApi("Lookups"));
    }

    public ValueTask<ApiResult<WorksheetStatusModel[]>> GetHistory(Guid worksheetId)
    {
        return Get<WorksheetStatusModel[]>(BuildApi("WorksheetHistory", new
        {
            worksheetId
        }));
    }

    public ValueTask<ApiResult<WorksheetStatusModel>> GetLatestHistory(Guid worksheetId)
    {
        return Get<WorksheetStatusModel>(BuildApi("WorksheetHistory/Latest", new
        {
            worksheetId
        }));
    }
}