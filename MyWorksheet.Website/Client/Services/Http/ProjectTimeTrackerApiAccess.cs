using System;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

namespace MyWorksheet.Website.Client.Services.Http;

public class ProjectTimeTrackerApiAccess : HttpAccessBase
{
    public ProjectTimeTrackerApiAccess(HttpService httpService) : base(httpService, "ProjectApi")
    {

    }

    public ValueTask<ApiResult<WorksheetTimeTrackerViewModel[]>> GetTrackers()
    {
        return Get<WorksheetTimeTrackerViewModel[]>(BuildApi("GetTrackers"));
    }

    public ValueTask<ApiResult<WorksheetTimeTrackerViewModel>> GetTracker(Guid worksheetId)
    {
        return Get<WorksheetTimeTrackerViewModel>(BuildApi("GetTracker", new
        {
            worksheetId
        }));
    }

    public ValueTask<ApiResult<WorksheetTimeTrackerViewModel>> StopTracker(Guid worksheetId, int minutesInPast = 0)
    {
        return Post<object, WorksheetTimeTrackerViewModel>(BuildApi("EndTrack", new
        {
            worksheetId,
            minutesInPast
        }), null);
    }

    public ValueTask<ApiResult<WorksheetTimeTrackerViewModel>> BeginTrack(Guid worksheetId)
    {
        return Post<object, WorksheetTimeTrackerViewModel>(BuildApi("EndTrack", new
        {
            worksheetId,
        }), null);
    }
}