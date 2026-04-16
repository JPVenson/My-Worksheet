using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

namespace MyWorksheet.Website.Client.Services.Http;

public class WorksheetItemApiAccess : RestHttpAccessBase<WorksheetItemModel, WorksheetItemModel>
{
    public WorksheetItemApiAccess(HttpService httpService)
        : base(httpService, "WorksheetItemsApi")
    {
    }


    public ValueTask<ApiResult<WorksheetItemModel[]>> GetByWorksheet(Guid worksheetId)
    {
        return Get<WorksheetItemModel[]>(BuildApi("GetByWorksheet", new
        {
            worksheetId
        }));
    }

    public ValueTask<ApiResult<MergeReportViewModel>> MergeReport(DateTimeOffset from, DateTimeOffset to, Guid worksheetId)
    {
        return Get<MergeReportViewModel>(BuildApi("MergeReport", new
        {
            from,
            to,
            worksheetId
        }));
    }

    public Task<IEnumerable<string>> CommentSuggestion(string text)
    {
        return GetValue<IEnumerable<string>>(BuildApi("GetWorksheetTexts", new
        {
            likeText = text ?? string.Empty
        })).AsTask();
    }
}