using System;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Workflow;

namespace MyWorksheet.Website.Client.Services.Http;

public class WorksheetWorkflowDataApiAccess : HttpAccessBase
{
    public WorksheetWorkflowDataApiAccess(HttpService httpService)
        : base(httpService, "WorksheetWorkflowData")
    {
    }

    public ValueTask<ApiResult<WorksheetWorkflowDataMapViewModel[]>> GetWorkflowData(Guid workflowId)
    {
        return Get<WorksheetWorkflowDataMapViewModel[]>(BuildApi("GetGroups", new
        {
            workflowId
        }));
    }

    public ValueTask<ApiResult<WorksheetWorkflowDataMapViewModel>> GetWorkflowDataGroup(Guid workflowDataGroupId)
    {
        return Get<WorksheetWorkflowDataMapViewModel>(BuildApi("GetGroup", new
        {
            workflowDataGroupId
        }));
    }

    public ValueTask<ApiResult<WorkflowDataViewModel>> GetValues(Guid workflowDataGroupId)
    {
        return Get<WorkflowDataViewModel>(BuildApi("GetValues", new
        {
            groupId = workflowDataGroupId
        }));
    }

    public ValueTask<ApiResult<WorkflowDataViewModel>> GetWorkflowStructure(Guid workflowId)
    {
        return Get<WorkflowDataViewModel>(BuildApi("GetStructure", new
        {
            workflowId
        }));
    }

    public ValueTask<ApiResult<WorksheetWorkflowDataMapViewModel>> Create(CreateWorksheetWorkflowDataMapViewModel groupData)
    {
        return Post<CreateWorksheetWorkflowDataMapViewModel, WorksheetWorkflowDataMapViewModel>(BuildApi("Create"), groupData);
    }

    public ValueTask<ApiResult> Update(WorksheetWorkflowDataMapViewModel groupDataMap)
    {
        return Post<WorksheetWorkflowDataMapViewModel>(BuildApi("Update"), groupDataMap);
    }
}