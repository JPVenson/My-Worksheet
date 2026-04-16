using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Shared;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.UserWorkload;
using MyWorksheet.Website.Client.Services.Workflow;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Storage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace MyWorksheet.Website.Client.Pages.TimeTracking.Worksheet;

public partial class WorksheetHistoryComponent
{
    public WorksheetHistoryComponent()
    {
        HistoryFiles = new Dictionary<Guid, IFutureList<FileModel>>();
    }

    [Parameter]
    public WorksheetEditViewModel Model { get; set; }
    [Inject]
    public HttpService HttpService { get; set; }
    [Inject]
    public WorkflowService WorkflowService { get; set; }

    public IFutureList<FileModel> LatestStateWorkflowFiles { get; set; }

    public IDictionary<Guid, IFutureList<FileModel>> HistoryFiles { get; set; }

    public IFutureList<FileModel> GetFilesForHistory(Guid historyId)
    {
        if (!HistoryFiles.TryGetValue(historyId, out var list))
        {
            async Task<ApiResult<FileModel[]>> Loader() =>
                (await HttpService.WorksheetWorkflowApiAccess.GetHistoryFiles(historyId))
                .With(f => f.Select(e => new FileModel(e)).ToArray());

            HistoryFiles[historyId] = list = new FutureList<FileModel>(Loader);
        }

        list.Load();
        return list;
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        WhenChanged(WorkflowService)
            .ThenRefresh(this);
        WhenChanged(Model.WorkflowHistory)
            .ThenRefresh(this)
            .Then(() =>
            {
                var lastHistory = Model.WorkflowHistory.FirstOrDefault();
                if (lastHistory == null)
                {
                    return;
                }
                LatestStateWorkflowFiles = GetFilesForHistory(lastHistory.WorksheetStatusHistoryId);
                LatestStateWorkflowFiles.WhenLoadedOnce(Render);
                LatestStateWorkflowFiles.Load();
            }).Trigger();

    }

    public async Task<FileModel[]> UploadFileToHistory(BrowserFile[] files, Guid storageProviderId, IProgressEx<int> progress)
    {
        var currentStep = Model.WorkflowHistory.First();
        var result = await HttpService.WorksheetWorkflowApiAccess.AddWorkflowFiles(currentStep.WorksheetStatusHistoryId, storageProviderId, files.Select(f => f.File).ToArray(), progress);

        if (result.Success)
        {
            return result.Object.Select(e => new FileModel(e.Object)).ToArray();
        }

        return null;
    }

    public async Task DeleteFileFromHistory(FileModel file, Guid historyId)
    {
        var deleteHistoryFiles = await HttpService.WorksheetWorkflowApiAccess.DeleteHistoryFiles(historyId, file.Model.StorageEntryId);

    }

    private async Task SaveWorkflow()
    {
        if (Model.WorkflowHistory.Count > 1)
        {
            return;
        }

        ServerErrorManager.Clear();
        using (WaiterService.WhenDisposed())
        {
            ServerErrorManager.Eval(await HttpService.WorksheetWorkflowApiAccess.SetWorkflow(
                Model.Worksheet.WorksheetId, Model.Workflow?.WorksheetWorkflowId,
                Model.WorkflowDataSet?.WorksheetWorkflowDataMapId).AsTask());
            ServerErrorManager.DisplayStatus();
            if (!ServerErrorManager.ServerErrors.Any())
            {
                Model.WorkflowActions.Reset();
                await Model.WorkflowActions.Load();
            }
        }
    }
}