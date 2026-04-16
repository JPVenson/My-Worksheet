using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Storage;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Workflow;
using Microsoft.AspNetCore.Components.Forms;

namespace MyWorksheet.Website.Client.Services.Http;

public class WorksheetWorkflowApiAccess : HttpAccessBase
{
    public WorksheetWorkflowApiAccess(HttpService httpService)
        : base(httpService, "WorksheetWorkflowApi")
    {
    }

    public ValueTask<ApiResult<WorksheetWorkflowStepTransitionViewModel[]>> GetWorkflowTransitions(Guid workflowId)
    {
        return Get<WorksheetWorkflowStepTransitionViewModel[]>(BuildApi("GetWorkflowTransitions", new
        {
            workflowId
        }));
    }

    public ValueTask<ApiResult<WorksheetWorkflowStepViewModel[]>> GetCurrentStepsForWorksheet(Guid worksheetId)
    {
        return Get<WorksheetWorkflowStepViewModel[]>(BuildApi("GetWorkflowsForWorksheet", new
        {
            worksheetId
        }));
    }

    public ValueTask<ApiResult<WorksheetWorkflowModel[]>> GetWorkflows()
    {
        return Get<WorksheetWorkflowModel[]>(BuildApi("GetWorkflows"));
    }

    public ValueTask<ApiResult> SetWorkflow(Guid worksheetId, Guid? workflowId, Guid? workflowDataId)
    {
        return Post(BuildApi("SetWorkflow", new
        {
            worksheetId,
            workflowId,
            workflowDataId
        }));
    }

    public ValueTask<ApiResult<JsonSchema>> GetDataForNextStep(Guid worksheetWorksheetId, Guid actionValue)
    {
        return Get<JsonSchema>(BuildApi("WorkflowDataInfo", new
        {
            worksheetId = worksheetWorksheetId,
            step = actionValue
        }));
    }

    public ValueTask<ApiResult> SetStatus(IDictionary<string, object> modelValues, Guid worksheetId, Guid statusId, string submitReason)
    {
        return Post(BuildApi("SetWorkflowStatus", new
        {
            worksheetId = worksheetId,
            statusId,
            reason = submitReason
        }), modelValues);
    }

    public ValueTask<ApiResult<StorageEntryViewModel[]>> GetHistoryFiles(Guid historyId)
    {
        return Get<StorageEntryViewModel[]>(BuildApi("GetWorkflowFiles", new
        {
            statusHistoryId = historyId
        }));
    }

    public ValueTask<ApiResult<string>> DeleteHistoryFiles(Guid historyId, Guid storageId)
    {
        return Post<object, string>(BuildApi("DeleteWorkflowFiles", new
        {
            statusHistoryId = historyId,
            storageEntryId = storageId
        }), null);
    }

    public ValueTask<ApiResult<StandardOperationResultBase<StorageEntryViewModel>[]>> AddWorkflowFiles(
        Guid historyId,
        Guid storageProviderId,
        IBrowserFile[] files,
        IProgressEx<int> progressEx = null)
    {
        return PostUpload<StandardOperationResultBase<StorageEntryViewModel>[]>(files, BuildApi("AddWorkflowFiles", new
        {
            statusHistoryId = historyId,
            storageProviderId = storageProviderId
        }));
    }
}