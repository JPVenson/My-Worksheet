using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Blob;
using MyWorksheet.Website.Server.Services.Blob.Provider;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Services.ObjectChanged;
using MyWorksheet.Website.Server.Services.WorksheetWorkflowManagment;
using MyWorksheet.Website.Server.Util.Auth;
using MyWorksheet.Website.Server.Util.Extentions;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Storage;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Workflow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Controllers.Api.WorksheetApi.Workflow;

[Route("api/WorksheetWorkflowApi")]
public class WorksheetWorkflowApiControllerBase : ApiControllerBase
{
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;
    private readonly IMapperService _mapper;
    private readonly WorksheetWorkflowManager _workflowManager;
    private readonly BlobManagerService _blobManagerService;
    private readonly ObjectChangedService _objectChangedService;

    public WorksheetWorkflowApiControllerBase(IDbContextFactory<MyworksheetContext> dbContextFactory,
        IMapperService mapper,
        WorksheetWorkflowManager workflowManager,
        BlobManagerService blobManagerService,
        ObjectChangedService objectChangedService)
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
        _workflowManager = workflowManager;
        _blobManagerService = blobManagerService;
        _objectChangedService = objectChangedService;
    }

    [HttpPost]
    [Route("DeleteWorkflowFiles")]
    public async Task<IActionResult> RemoveWorkflowFile(Guid statusHistoryId, Guid storageEntryId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var entry = db.WorksheetWorkflowStorageMaps.FirstOrDefault(e => e.IdWorksheetStatusHistory == statusHistoryId && e.IdStorageEntry == storageEntryId);
        if (entry == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var worksheetStatusHistory = db.WorksheetStatusHistories.Find(entry.IdWorksheetStatusHistory);
        var worksheet = db.Worksheets.Find(worksheetStatusHistory.IdWorksheet);
        var storageEntry = db.StorageEntries.Find(entry.IdStorageEntry);
        if (worksheet.IdCreator != User.GetUserId())
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }
        var storageDeletedFromExternal = true;
        try
        {
            await _blobManagerService.DeleteData(entry.IdStorageEntry, User.GetUserId());
        }
        catch (Exception)
        {
            storageDeletedFromExternal = false;
        }

        db.WorksheetWorkflowStorageMaps.Remove(entry);
        db.SaveChanges();
        if (storageDeletedFromExternal)
        {
            return Data();
        }
        var storageProvider = db.StorageProviders.Find(storageEntry.IdStorageProvider);
        return Data($"Removed File from My-Worksheet but could not delete it from Storage Provider: {storageProvider.Name}. The filename was:" +
                    $"'{storageEntry.StorageKey}'.\r\n" +
                    $"You might have to delete it by yourself.");
    }

    [HttpGet]
    [Route("GetWorkflowFiles")]
    public IActionResult GetWorkflowFiles(Guid statusHistoryId)
    {
        using var db = _dbContextFactory.CreateDbContext();

        var historyEntry = db.WorksheetStatusHistories.FirstOrDefault(e => e.IdChangeUser == User.GetUserId() && e.WorksheetStatusHistoryId == statusHistoryId);
        if (historyEntry == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var filesMap = db.WorksheetWorkflowStorageMaps.Where(e => e.IdWorksheetStatusHistory == statusHistoryId).Select(e => e.IdStorageEntryNavigation).ToArray();
        if (!filesMap.Any())
        {
            return Data();
        }
        return Data(_mapper.ViewModelMapper.Map<StorageEntryViewModel[]>(filesMap));
    }

    [HttpPost]
    [Route("AddWorkflowFiles")]
    public async Task<IActionResult> AddWorkflowFiles(Guid statusHistoryId,
        Guid storageProviderId,
        IFormFileCollection fileCollection)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var storageProvider = db.StorageProviders.FirstOrDefault(e => (e.IdAppUser == null || e.IdAppUser == User.GetUserId()) && e.StorageProviderId == storageProviderId);

        if (storageProvider == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var historyEntry = db.WorksheetStatusHistories.FirstOrDefault(e => e.WorksheetStatusHistoryId == statusHistoryId && e.IdChangeUser == User.GetUserId());

        if (historyEntry == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var files = fileCollection.Select(file =>
            {
                return _blobManagerService.SetData(
                    new BlobDataCore(file),
                    storageProvider.StorageProviderId, User.GetUserId(),
                    StorageEntityType.WorksheetDocument);
            })
            .ToArray();

        var createdFiles = new List<BlobManagerSetOperationResult>();
        foreach (var file in files)
        {
            var storageEntry = await file;
            createdFiles.Add(storageEntry);
            if (storageEntry.Success)
            {
                db.Add(new WorksheetWorkflowStorageMap()
                {
                    IdStorageEntry = storageEntry.Object.StorageEntryId,
                    IdWorksheetStatusHistory = statusHistoryId,
                    IdAppUser = User.GetUserId()
                });
            }
        }

        db.SaveChanges();
        return Data(_mapper.ViewModelMapper.Map<StandardOperationResultBase<StorageEntryViewModel>[]>(createdFiles));
    }

    [HttpGet]
    [Route("GetWorkflowTransitions")]
    public IActionResult GetWorkflowTransitions(Guid workflowId)
    {
        using var db = _dbContextFactory.CreateDbContext();

        var realId = db.WorksheetWorkflows.Find(workflowId);
        if (realId == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var worksheetWorkflowStatus = _workflowManager.GetWorkflowTransitions(db, realId.WorksheetWorkflowId);
        return Data(worksheetWorkflowStatus.Select(e => new WorksheetWorkflowStepTransitionViewModel()
        {
            From = _mapper.ViewModelMapper.Map<WorksheetWorkflowStepViewModel>(e.Key),
            To = _mapper.ViewModelMapper.Map<WorksheetWorkflowStepViewModel[]>(e.Value)
        }));
    }

    [HttpGet]
    [Route("GetWorkflowsForWorksheet")]
    public IActionResult GetWorkflowsForWorksheet(Guid worksheetId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var realId = db.Worksheets.FirstOrDefault(e => e.IdCreator == User.GetUserId() && e.WorksheetId == worksheetId);

        if (realId == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var worksheetWorkflowStatus = _workflowManager.GetPossibleWorkflowStep(db, realId.IdWorksheetWorkflow, realId.IdCurrentStatus);
        return Data(_mapper.ViewModelMapper.Map<WorksheetWorkflowStepViewModel[]>(worksheetWorkflowStatus));
    }

    [HttpGet]
    [Route("WorkflowDataInfo")]
    public async Task<IActionResult> GetWorkflowInfo(Guid worksheetId, Guid step)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var realId = db.Worksheets.FirstOrDefault(e => e.IdCreator == User.GetUserId() && e.WorksheetId == worksheetId);
        if (realId == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var worksheetWorkflowStatus = await _workflowManager.GetPossibleWorkflowStepInfo(db, realId.IdWorksheetWorkflow, step, User.GetUserId(), worksheetId);
        return Data(worksheetWorkflowStatus);
    }

    [HttpGet]
    [Route("GetWorkflows")]
    public IActionResult GetWorkflows()
    {
        using var db = _dbContextFactory.CreateDbContext();
        return Data(_mapper.ViewModelMapper.Map<WorksheetWorkflowModel[]>(db.WorksheetWorkflows.ToArray().Where(e => _workflowManager.WorksheetWorkflows.ContainsKey(e.WorksheetWorkflowId))));
    }

    [HttpPost]
    [Route("SetWorkflow")]
    public async Task<IActionResult> SetWorkflow([FromQuery] Guid worksheetId,
        [FromQuery] Guid workflowid,
        [FromQuery] Guid? workflowDataId = null)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var workflow = db.Worksheets.FirstOrDefault(e => e.IdCreator == User.GetUserId() && e.WorksheetId == worksheetId);
        if (workflow == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        if (workflow.IdWorksheetWorkflow.HasValue)
        {
            if (!_workflowManager.GetCanModify(workflow.IdWorksheetWorkflow.Value, workflow.IdCurrentStatus))
            {
                return BadRequest("You cannot change the Workflow of an Worksheet when a status change was done");
            }
        }

        IWorksheetWorkflow wf;
        if (!_workflowManager.WorksheetWorkflows.TryGetValue(workflowid, out wf))
        {
            return BadRequest("Invalid Workflow id");
        }

        if (wf.NeedsCData && !workflowDataId.HasValue)
        {
            return BadRequest("This workflow requires an set of Workflow Data");
        }

        //if (workflowDataId.HasValue)
        //{
        //	var workflowDataSchema = wf.GetSchema(_db, User.GetUserId());
        //	if (workflowDataSchema != JsonSchema.EmptyNotNull)
        //	{
        //		var dataSet = _db.WorksheetWorkflowDataMaps.Where
        //			.Column(f => f.WorksheetWorkflowDataMapId).Is.EqualsTo(workflowDataId.Value)
        //			.And
        //			.Column(f => f.IdCreator).Is.EqualsTo(User.GetUserId())
        //			.And
        //			.Column(f => f.IdWorksheetWorkflow).Is.EqualsTo(workflowid)
        //			.FirstOrDefault();

        //		if (dataSet == null)
        //		{
        //			return BadRequest("The selected WorkflowData does not match the given Workflow");
        //		}

        //		var valuesFromDb = _db.WorksheetWorkflowDatas
        //			.Where
        //			.Column(f => f.WorksheetWorkflowDataId).Is.EqualsTo(dataSet.WorksheetWorkflowDataMapId)
        //			.ToArray()
        //			.ToDictionary(e => e.Key, e => e.Value);

        //		var dataValues = JsonHelper.MapToSchema(valuesFromDb, workflowDataSchema, _db.Config, JsonSchema.Delimiter)
        //			.Expand();
        //		var validationResult = workflowDataSchema.Validate(dataValues, _db.Config);
        //		if (validationResult.Any())
        //		{
        //			return BadRequest("You cannot run this Workflow with the selected data as the Validation failed: " + validationResult.Select(e =>
        //									  $"'{e.Key}' is {e.Value}")
        //								  .Aggregate((e, f) => e + " and " + f));
        //		}
        //	}
        //}

        db.Worksheets.Where(e => e.WorksheetId == workflow.WorksheetId)
            .ExecuteUpdate(e => e.SetProperty(f => f.IdWorksheetWorkflow, workflowid).SetProperty(f => f.IdWorksheetWorkflowDataMap, workflowDataId));
        await _objectChangedService.SendObjectChanged(ChangeEventTypes.Changed, typeof(Models.Worksheet), workflow.WorksheetId, Request.GetSignalId(), User.GetUserId());
        return Data();
    }

    [HttpPost]
    [Route("SetWorkflowStatus")]
    public async Task<IActionResult> SetWorkflowStatus([FromBody] IDictionary<string, object> additonalData,
        [FromQuery] Guid worksheetId,
        [FromQuery] Guid statusId,
        [FromQuery] string reason = null)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var worksheet = db.Worksheets.FirstOrDefault(e => e.IdCreator == User.GetUserId() && e.WorksheetId == worksheetId);

        if (worksheet == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var data = additonalData ?? new Dictionary<string, object>();
        //data = data.ToDictionary(e => e.Key, e => e.Value is JsonElement)

        var workflowResult = await _workflowManager.SetWorksheetWorkflowStep(db, worksheet, statusId, User.GetUserId(), reason, data, worksheet.IdWorksheetWorkflowDataMap);

        if (!workflowResult)
        {
            return BadRequest(workflowResult.Reason.AsTranslation());
        }

        return Data();
    }
}