using System;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Storage;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Asserts;
using Microsoft.AspNetCore.Components.Forms;

namespace MyWorksheet.Website.Client.Services.Http;

public class WorksheetAssertApiAccess : RestHttpAccessBase<WorksheetAssertCreateViewModel, WorksheetAssertCreateViewModel>
{
    public WorksheetAssertApiAccess(HttpService httpService) : base(httpService, "WorksheetAssertApi")
    {
    }

    public ValueTask<ApiResult<WorksheetAssertCreateViewModel[]>> GetAssertsFromWorksheet(Guid worksheetId)
    {
        return Get<WorksheetAssertCreateViewModel[]>(BuildApi("GetForWorksheet", new
        {
            worksheetId
        }));
    }

    public ValueTask<ApiResult<StorageEntryViewModel[]>> GetStorageEntries(Guid worksheetAssertId)
    {
        return Get<StorageEntryViewModel[]>(BuildApi("GetStorageEntries", new
        {
            worksheetAssertId
        }));
    }

    public ValueTask<ApiResult<WorksheetAssertCreateViewModel[]>> GetForProject(Guid projectId)
    {
        return Get<WorksheetAssertCreateViewModel[]>(BuildApi("GetForProject", new
        {
            projectId
        }));
    }

    public ValueTask<ApiResult> RemoveFile(Guid storageEntryId)
    {
        return Post(BuildApi("Delete/File", new
        {
            storageEntryId
        }));
    }

    public ValueTask<ApiResult<StandardOperationResultBase<StorageEntryViewModel>[]>> AddFile(Guid worksheetAssertId, Guid storageProviderId, IBrowserFile[] files)
    {
        return PostUpload<StandardOperationResultBase<StorageEntryViewModel>[]>(files, BuildApi("Create/FileUpload", new
        {
            worksheetAssertId,
            storageProviderId,
        }));
    }
}