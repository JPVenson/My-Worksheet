using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Shared.WebApi;
using MyWorksheet.Webpage.Helper.Roles;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Blob;
using MyWorksheet.Website.Server.Services.Blob.Provider;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Services.ObjectChanged;
using MyWorksheet.Website.Server.Util.Auth;
using MyWorksheet.Website.Server.Util.Extentions;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Storage;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Asserts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Controllers.Api.WorksheetApi;

[Route("api/WorksheetAssertApi")]
[RevokableAuthorize(Roles =
    Roles.AdminRoleName + "," + Roles.WorksheetAdminRoleName + "," + Roles.WorksheetUserRoleName)]
public class WorksheetAssertControllerBase : RestApiControllerBase<WorksheetAssert, WorksheetAssertCreateViewModel, WorksheetAssertCreateViewModel>
{
    private readonly IBlobManagerService _blobManagerService;
    private readonly ObjectChangedService _objectChangedService;

    public WorksheetAssertControllerBase(
        IDbContextFactory<MyworksheetContext> dbContextFactory,
        IMapperService mapper,
        IBlobManagerService blobManagerService,
        ObjectChangedService objectChangedService)
        : base(dbContextFactory, mapper)
    {
        _blobManagerService = blobManagerService;
        _objectChangedService = objectChangedService;
    }

    [HttpGet]
    [Route("GetForWorksheet")]
    public IActionResult GetForWorksheet(Guid worksheetId)
    {
        var db = EntitiesFactory.CreateDbContext();
        return Data(MapperService.ViewModelMapper.Map<WorksheetAssertGetViewModel[]>(db.WorksheetAsserts
            .Where(f => f.IdAppUser == User.GetUserId())
            .Where(f => f.IdWorksheet == worksheetId)
            .ToArray()));
    }

    [HttpGet]
    [Route("GetStorageEntries")]
    public IActionResult GetEntries(Guid worksheetAssertId)
    {
        var db = EntitiesFactory.CreateDbContext();
        var assert = db.WorksheetAsserts.Find(worksheetAssertId);

        if (assert == null || assert.IdAppUser != User.GetUserId())
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var maps = db.WorksheetAssertsFilesMaps
            .Where(f => f.IdWorksheetAssert == worksheetAssertId)
            .ToArray();
        if (!maps.Any())
        {
            return Data();
        }

        return Data(MapperService.ViewModelMapper.Map<StorageEntryViewModel[]>(db.StorageEntries.Where(e => maps.Select(e => e.IdStorageEntry).Contains(e.StorageEntryId))));
    }

    [HttpGet]
    [Route("GetForProject")]
    public IActionResult GetForProject(Guid projectId)
    {
        var db = EntitiesFactory.CreateDbContext();
        return Data(MapperService.ViewModelMapper.Map<WorksheetAssertGetViewModel[]>(db.WorksheetAsserts
            .Where(f => f.IdAppUser == User.GetUserId())
            .Where(f => f.IdProject == projectId)
            .ToArray()));
    }

    [HttpPost]
    [Route("Delete")]
    public async Task<IActionResult> RemoveAssert(Guid id)
    {
        var db = EntitiesFactory.CreateDbContext();
        var assert = db.WorksheetAsserts.Find(id);

        if (assert == null || assert.IdAppUser != User.GetUserId())
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var worksheetAsserts = db.WorksheetAssertsFilesMaps
            .Where(f => f.IdWorksheetAssert == assert.WorksheetAssertId)
            .ToArray();

        foreach (var worksheetAssertsFilesMap in worksheetAsserts)
        {
            await _blobManagerService.DeleteData(worksheetAssertsFilesMap.IdStorageEntry, User.GetUserId());
        }
        db.WorksheetAssertsFilesMaps.Where(e => e.IdWorksheetAssert == id).ExecuteDelete();
        db.WorksheetAsserts.Where(e => e.WorksheetAssertId == id).ExecuteDelete();

        await _objectChangedService.SendObjectChanged(ChangeEventTypes.Removed, assert, Request.GetSignalId(), User.GetUserId());
        return Data();
    }

    [HttpPost]
    [Route("Delete/File")]
    public async Task<IActionResult> RemoveFile(Guid storageEntryId)
    {
        var db = EntitiesFactory.CreateDbContext();
        var worksheetAssertsFilesMap = db.WorksheetAssertsFilesMaps
            .Where(f => f.IdStorageEntry == storageEntryId).FirstOrDefault();

        if (worksheetAssertsFilesMap == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var assert = db.WorksheetAsserts.Find(worksheetAssertsFilesMap.IdWorksheetAssert);
        if (assert.IdAppUser != User.GetUserId())
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var storageEntry = db.StorageEntries.Find(worksheetAssertsFilesMap.IdStorageEntry);
        var storageDeletedFromExternal = true;
        try
        {
            await _blobManagerService.DeleteData(worksheetAssertsFilesMap.IdStorageEntry, User.GetUserId());
        }
        catch (Exception)
        {
            storageDeletedFromExternal = false;
        }

        db.WorksheetAssertsFilesMaps.Where(e => e.WorksheetAssertsFilesMapId == worksheetAssertsFilesMap.WorksheetAssertsFilesMapId).ExecuteDelete();
        await _objectChangedService.SendObjectChanged(ChangeEventTypes.Removed, storageEntry, Request.GetSignalId(), User.GetUserId());
        if (storageDeletedFromExternal)
        {
            return Data();
        }
        var storageProvider = db.StorageProviders.Find(storageEntry.IdStorageProvider);
        return Data($"Removed File from My-Worksheet but could not delete it from Storage Provider: {storageProvider.Name}. The filename was:" +
                    $"'{storageEntry.StorageKey}'.\r\n" +
                    $"You might have to delete it by yourself.");
    }

    public override async ValueTask<IActionResult> Update(WorksheetAssertCreateViewModel model, Guid id)
    {
        var db = EntitiesFactory.CreateDbContext();
        var assert = db.WorksheetAsserts.Find(id);

        if (assert == null || assert.IdAppUser != User.GetUserId())
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        model.IdProject = assert.IdProject;
        model.IdWorksheet = assert.IdWorksheet;

        if (CheckAssetForInvalidAccess(model, db, out var unauthorized))
        {
            return unauthorized;
        }

        assert = MapperService.ViewModelMapper.Map(model, assert);
        db.Update(assert);
        await _objectChangedService.SendObjectChanged(ChangeEventTypes.Changed, assert, Request.GetSignalId(), User.GetUserId());
        return Data(model);
    }

    public override async ValueTask<IActionResult> Create(WorksheetAssertCreateViewModel model)
    {
        var db = EntitiesFactory.CreateDbContext();
        if (CheckAssetForInvalidAccess(model, db, out var unauthorized))
        {
            return unauthorized;
        }

        var assert = MapperService.ViewModelMapper.Map<WorksheetAssert>(model);
        assert.IdAppUser = User.GetUserId();
        db.Add(assert);
        db.SaveChanges();
        await _objectChangedService.SendObjectChanged(ChangeEventTypes.Added, assert, Request.GetSignalId(), User.GetUserId());
        return Data(MapperService.ViewModelMapper.Map<WorksheetAssertCreateViewModel>(assert));
    }

    private bool CheckAssetForInvalidAccess(WorksheetAssertCreateViewModel model, MyworksheetContext db,
        out IActionResult resultCode)
    {
        if (!model.IdProject.HasValue && !model.IdWorksheet.HasValue)
        {
            resultCode = BadRequest("WorksheetAssert/NoTargetSpecified".AsTranslation());
            return true;
        }

        if (model.IdWorksheet.HasValue)
        {
            var worksheet = db.Worksheets.Find(model.IdWorksheet.Value);
            if (worksheet == null)
            {
                {
                    resultCode = Unauthorized("Common/InvalidId".AsTranslation());
                    return true;
                }
            }

            if (worksheet.IdCreator != User.GetUserId())
            {
                {
                    resultCode = Unauthorized("Common/InvalidId".AsTranslation());
                    return true;
                }
            }

            if (model.Tax < 0)
            {
                {
                    resultCode = Unauthorized("Common/InvalidId".AsTranslation());
                    return true;
                }
            }
        }

        if (model.IdProject.HasValue)
        {
            var project = db.Projects.Find(model.IdProject.Value);
            if (project == null)
            {
                {
                    resultCode = Unauthorized("Common/InvalidId".AsTranslation());
                    return true;
                }
            }

            if (project.IdCreator != User.GetUserId())
            {
                {
                    resultCode = Unauthorized("Common/InvalidId".AsTranslation());
                    return true;
                }
            }

            if (model.Tax < 0)
            {
                {
                    resultCode = Unauthorized("Common/InvalidId".AsTranslation());
                    return true;
                }
            }
        }

        resultCode = null;
        return false;
    }

    [HttpPost]
    [Route("Create/FileUpload")]
    public async Task<IActionResult> CreateWorksheetAssert(Guid worksheetAssertId, Guid storageProviderId,
        IFormFileCollection fileCollection)
    {
        var db = EntitiesFactory.CreateDbContext();
        var assert = db.WorksheetAsserts.Find(worksheetAssertId);

        if (assert == null || assert.IdAppUser != User.GetUserId())
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var storageProvider = db.StorageProviders.Where(e => e.IdAppUser == null || e.IdAppUser == User.GetUserId())
            .Where(e => e.StorageProviderId == storageProviderId)
            .FirstOrDefault();

        if (storageProvider == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }


        var files = fileCollection.Select(file => _blobManagerService.SetData(
                new BlobDataCore(file),
                storageProvider.StorageProviderId, User.GetUserId(),
                StorageEntityType.WorksheetDocument))
            .ToArray();

        var createdFiles = new List<BlobManagerSetOperationResult>();
        foreach (var file in files)
        {
            var storageEntry = await file;
            createdFiles.Add(storageEntry);
            if (storageEntry.Success)
            {
                db.Add(new WorksheetAssertsFilesMap()
                {
                    IdStorageEntry = storageEntry.Object.StorageEntryId,
                    IdWorksheetAssert = assert.WorksheetAssertId,
                });
            }
        }
        db.SaveChanges();

        await _objectChangedService.SendObjectChanged(ChangeEventTypes.Added, assert, Request.GetSignalId(), User.GetUserId());
        return Data(MapperService.ViewModelMapper.Map<StandardOperationResultBase<StorageEntryViewModel>[]>(createdFiles));
    }
}