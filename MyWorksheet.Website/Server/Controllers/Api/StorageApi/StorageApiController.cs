using System.Collections.Generic;
using System;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using MyWorksheet.Shared.WebApi;
using MyWorksheet.Website.Server.Helper;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Blob;
using MyWorksheet.Website.Server.Services.Blob.Thumbnail;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Shared.ObjectSchema;
using MyWorksheet.Website.Server.Util.Auth;
using MyWorksheet.Website.Shared.Util;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Controllers.Api.StorageApi;

[Route("api/StorageApi")]
[RevokableAuthorize]
public class StorageApiControllerBase : RestApiControllerBase<StorageProvider, StorageProviderViewModel, CreateStorageProvider>
{
    private readonly BlobManagerService _blobService;
    private readonly ITempFileTokenService _tempFileTokenService;
    private readonly IBlobManagerService _blobManagerService;

    public StorageApiControllerBase(BlobManagerService blobService, IMapperService mapper, IDbContextFactory<MyworksheetContext> dbContextFactory,
        ITempFileTokenService tempFileTokenService,
        IBlobManagerService blobManagerService) : base(dbContextFactory, mapper)
    {
        _blobService = blobService;
        _tempFileTokenService = tempFileTokenService;
        _blobManagerService = blobManagerService;
    }


    [AllowAnonymous]
    [Route("Download")]
    [HttpGet]
    public async Task<IActionResult> DownloadFromStorage(string token, Guid storageEntryId)
    {
        var tokenInfo = _tempFileTokenService.RedeemToken(token, storageEntryId, Request.HttpContext.Connection.RemoteIpAddress.ToString(), true);
        if (tokenInfo == null)
        {
            return Redirect("/error");
        }

        using var db = EntitiesFactory.CreateDbContext();
        var storageEntry = db.StorageEntries.Find(tokenInfo.Value.StorageId);
        var getFilestream = await _blobService.GetData(tokenInfo.Value.StorageId, tokenInfo.Value.UserId);

        if (getFilestream?.Object != null)
        {
            Response.RegisterForDispose(getFilestream.Object);
        }

        if (getFilestream == null || !getFilestream.Success)
        {
            return Redirect("/error");
        }

        return new FileStreamResult(getFilestream.Object, storageEntry.ContentType)
        {
            EnableRangeProcessing = false,
        };

        //result.Content.Headers.ContentDisposition =
        //		new ContentDispositionHeaderValue(DispositionTypeNames.Attachment)
        //		{
        //			FileName = storageEntry.FileName,
        //			CreationDate = DateTimeOffset.UtcNow,
        //		};
    }

    [RevokableAuthorize]
    [Route("CreateTokenForStorageEntry")]
    [HttpGet]
    public IActionResult CreateTokenForStorageEntry(Guid storageEntryId)
    {
        return Data(_tempFileTokenService.IssueFileToken(User.GetUserId(), storageEntryId,
            Request.HttpContext.Connection.RemoteIpAddress.ToString()));
    }

    [RevokableAuthorize]
    [Route("GetStorageProviderTypes")]
    [HttpGet]
    public IActionResult GetStorageProviderTypes()
    {
        return Data(MapperService.ViewModelMapper.Map<StorageProviderInfo[]>(_blobService.BlobProvidersFactory));
    }

    [RevokableAuthorize]
    [Route("GetStorageTypes")]
    [HttpGet]
    public IActionResult GetStorageTypes()
    {
        using var db = EntitiesFactory.CreateDbContext();
        return Data(MapperService.ViewModelMapper.Map<StorageTypeViewModel[]>(db.StorageTypes));
    }

    public override IActionResult GetSingle(Guid id)
    {
        using var db = EntitiesFactory.CreateDbContext();
        var storageEntry = db.StorageEntries.Find(id);

        if (storageEntry == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var providerForEntry = db.StorageProviders.Where(e => (e.IdAppUser == null || e.IdAppUser == User.GetUserId()) && e.StorageProviderId == storageEntry.IdStorageProvider).FirstOrDefault();
        if (providerForEntry == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        return Data(MapperService.ViewModelMapper.Map<StorageEntryViewModel>(storageEntry));
    }

    [AllowAnonymous]
    [Route("GetThumbnail")]
    [HttpGet]
    public async Task<IActionResult> GetThumbnail(Guid storageId, string size, string token)
    {
        var redeemToken = _tempFileTokenService.RedeemToken(token, storageId, Request.HttpContext.Connection.RemoteIpAddress.ToString(), false);
        if (!redeemToken.HasValue)
        {
            return Redirect("/error");
        }

        var thumbnail = await _blobManagerService.GetThumbnailAsync(storageId, redeemToken.Value.UserId, size);

        if (thumbnail?.Object != null)
        {
            Response.RegisterForDispose(thumbnail.Object);
        }

        if (thumbnail == null || !thumbnail.Success)
        {
            return Redirect("/error");
        }

        return File(thumbnail.Object, MimeKit.MimeTypes.GetMimeType(".jpeg"), false);
        //result.Content.Headers.ContentDisposition =
        //	new ContentDispositionHeaderValue(DispositionTypeNames.Inline)
        //	{
        //		FileName = size + "_thumbnail.jpeg",
        //		CreationDate = DateTimeOffset.UtcNow,
        //	};
    }

    [RevokableAuthorize]
    [Route("GetStorageEntries")]
    [HttpGet]
    public IActionResult GetStorageEntries(Guid storageProviderId, int page, int pageSize,
        bool? showDeleted = null,
        string searchText = null,
        Guid? ofType = null)
    {
        using var db = EntitiesFactory.CreateDbContext();
        var provider = db.StorageProviders.Where(e => (e.IdAppUser == null || e.IdAppUser == User.GetUserId()) && e.StorageProviderId == storageProviderId);

        if (provider == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var searchQuery = db.StorageEntries.Where(e => e.IdStorageProvider == storageProviderId);

        if (searchText != null)
        {
            searchQuery = searchQuery.Where(e => e.FileName.Contains(searchText));
        }

        if (showDeleted.HasValue)
        {
            searchQuery = searchQuery.Where(e => e.IsDeleted == showDeleted.Value);
        }

        if (ofType.HasValue)
        {
            searchQuery = searchQuery.Where(e => e.IdStorageType == ofType.Value);
        }

        return Data(MapperService.ViewModelMapper.Map<PageResultSet<StorageEntryViewModel>>(
            searchQuery.OrderBy(e => e.StorageEntryId).ForPagedResult(page, pageSize)
        ));
    }

    [RevokableAuthorize]
    [Route("GetStorageProviders")]
    [HttpGet]
    public IActionResult GetStorageProviders()
    {
        using var db = EntitiesFactory.CreateDbContext();
        var data = MapperService.ViewModelMapper.Map<GetStorageProvider[]>(db
        .StorageProviders.Where(e => e.IdAppUser == null || e.IdAppUser == User.GetUserId()).ToArray());

        return Data(data);
    }

    [RevokableAuthorize]
    [Route("GetStorageProvider")]
    [HttpGet]
    public IActionResult GetStorageProvider(Guid id)
    {
        using var db = EntitiesFactory.CreateDbContext();
        var data = MapperService.ViewModelMapper.Map<GetStorageProvider>(
            db.StorageProviders.Where(e => e.IdAppUser == null || e.IdAppUser == User.GetUserId())
            .Where(e => e.StorageProviderId == id)
            .FirstOrDefault());

        return Data(data);
    }

    [RevokableAuthorize]
    [Route("GetStorageProviderData")]
    [HttpGet]
    public IActionResult GetStorageProviderData(Guid storageId)
    {
        using var db = EntitiesFactory.CreateDbContext();
        var provider = db.StorageProviders
            .Include(e => e.StorageProviderData)
            .Where(e => e.IdAppUser == null || e.IdAppUser == User.GetUserId())
            .Where(e => e.StorageProviderId == storageId)
            .FirstOrDefault();

        if (provider == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }
        var data = MapperService.ViewModelMapper.Map<StorageProviderDataViewModel[]>(provider.StorageProviderData.ToArray());

        return Data(data.ToDictionary(e => e.Key, e => e.Value));
    }

    [RevokableAuthorize]
    [Route("GetStorageProviderDataStructure")]
    [HttpGet]
    public IActionResult GetStorageProviderDataStructure(string storageKey)
    {
        var dataStructure = _blobService.GetDataSturcture(storageKey);
        return Data(dataStructure);
    }

    [RevokableAuthorize]
    [Route("GetStorageProviderStatistics")]
    [HttpGet]
    public async Task<IActionResult> GetStorageProviderStatistics(Guid storageId)
    {
        var statistics = await _blobService.GetDataStatistics(storageId, User.GetUserId());

        if (statistics == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }
        return Data(statistics);
    }

    [RevokableAuthorize]
    [Route("TestProvider")]
    [HttpPost]
    public async Task<IActionResult> TestProvider(string storageKey, [FromBody] IDictionary<string, object> fields)
    {
        var providerToCreate =
            _blobService.BlobProvidersFactory.FirstOrDefault(e => e.Key.Equals(storageKey));

        if (providerToCreate == null)
        {
            return BadRequest("Invalid Provider Key");
        }

        var dataFields = fields.Select(e => new StorageProviderData()
        {
            IdAppUser = User.GetUserId(),
            IdStorageProvider = Guid.Empty,
            StorageProviderDataId = Guid.Empty,
            Key = e.Key,
            Value = e.Value?.ToString()
        }).ToArray();

        var providerInstance = providerToCreate.StorageProviderFactory(Guid.Empty, dataFields);
        var log = await providerInstance.Test(User.GetUserId());
        return Data(log);
    }

    [RevokableAuthorize]
    [Route("Delete")]
    [HttpPost]
    public IActionResult Delete(Guid id)
    {
        using var db = EntitiesFactory.CreateDbContext();
        var provider = db.StorageProviders
            .Where(e => e.IdAppUser == null || e.IdAppUser == User.GetUserId())
            .Where(e => e.StorageProviderId == id)
            .FirstOrDefault();
        if (provider?.IdAppUser == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        using var transaction = db.Database.BeginTransaction();
        db.StorageProviderData.Where(e => e.IdStorageProvider == provider.StorageProviderId).ExecuteDelete();
        db.StorageProviders.Remove(provider);
        db.SaveChanges();
        transaction.Commit();
        return Data();
    }

    public override async ValueTask<IActionResult> Create(CreateStorageProvider model)
    {
        var providerToCreate =
            _blobService.BlobProvidersFactory.FirstOrDefault(e => e.Key.Equals(model.ProviderInfo.StorageKey));

        if (providerToCreate == null)
        {
            return BadRequest("Invalid Provider Key");
        }

        StorageProvider provider = null;
        var objectSchema = providerToCreate.StorageProviderFactory(Guid.Empty, new StorageProviderData[0]).GetSchema();
        var db = await EntitiesFactory.CreateDbContextAsync();
        await using (db.ConfigureAwait(false))
        {
            var validateFields =
            objectSchema.Validate(model.Fields.ToDictionary(e => e.Key, e => e.Value));
            if (validateFields.Any())
            {
                return BadRequest(validateFields.Select(e => e.Key + ":" + e.Value.ToString()).Aggregate((e, f) => e + ", " + f));
            }

            var transaction = await db.Database.BeginTransactionAsync();
            await using (transaction.ConfigureAwait(false))
            {
                provider = MapperService.ViewModelMapper.Map<StorageProvider>(model.ProviderInfo);
                provider.IdAppUser = User.GetUserId();
                if (provider.IsDefaultProvider)
                {
                    await db.StorageProviders.Where(e => e.IdAppUser == provider.IdAppUser)
                    .ExecuteUpdateAsync(f => f.SetProperty(e => e.IsDefaultProvider, false)).ConfigureAwait(false);
                }
                db.Add(provider);

                foreach (var field in MapperService.ViewModelMapper.Map<StorageProviderData[]>(model.Fields))
                {
                    field.IdAppUser = User.GetUserId();
                    field.IdStorageProviderNavigation = provider;
                    if (objectSchema.Properties[field.Key].Type == "string | secure")
                    {
                        field.Value = ChallangeUtil.EncryptPassword(field.Value, User.Identity.Name);
                    }
                    db.Add(field);
                }

                await db.SaveChangesAsync().ConfigureAwait(false);
                transaction.Commit();
            }
        }
        return Data(MapperService.ViewModelMapper.Map<GetStorageProvider>(provider));
    }

    public override async ValueTask<IActionResult> Update(CreateStorageProvider model, Guid id)
    {
        var providerToCreate =
            _blobService.BlobProvidersFactory.FirstOrDefault(e => e.Key.Equals(model.ProviderInfo.StorageKey));

        if (providerToCreate == null)
        {
            return BadRequest("Invalid Provider Key");
        }

        var objectSchema = providerToCreate.StorageProviderFactory(Guid.Empty, new StorageProviderData[0]).GetSchema();
        var db = await EntitiesFactory.CreateDbContextAsync();
        await using (db.ConfigureAwait(false))
        {
            var validateFields =
                        objectSchema.Validate(model.Fields.ToDictionary(e => e.Key, e => e.Value));
            if (validateFields.Any())
            {
                return BadRequest(validateFields.Select(e => e.Key + ":" + e.Value.ToString()).Aggregate((e, f) => e + ", " + f));
            }

            var existingProvider = db.StorageProviders.Where(e => e.IdAppUser == User.GetUserId())
                .Where(e => e.StorageProviderId == model.ProviderInfo.StorageProviderId)
                .FirstOrDefault();
            if (existingProvider?.IdAppUser == null)
            {
                return Unauthorized("Common/InvalidId".AsTranslation());
            }

            var transaction = await db.Database.BeginTransactionAsync();
            await using (transaction.ConfigureAwait(false))
            {
                if (model.ProviderInfo.IsDefaultProvider && !existingProvider.IsDefaultProvider)
                {
                    await db.StorageProviders.Where(e => e.IdAppUser == existingProvider.IdAppUser)
                    .ExecuteUpdateAsync(f => f.SetProperty(e => e.IsDefaultProvider, false)).ConfigureAwait(false);
                }

                db.Update(MapperService.ViewModelMapper.Map(model.ProviderInfo, existingProvider));
                foreach (var field in MapperService.ViewModelMapper.Map<StorageProviderData[]>(model.Fields))
                {
                    var existingField = db.StorageProviderData.FirstOrDefault(e => e.IdStorageProvider == existingProvider.StorageProviderId && e.Key == field.Key);
                    if (existingField.Value.Equals(field.Value))
                    {
                        continue;
                    }
                    existingField.Value = field.Value;
                    if (objectSchema.Properties[field.Key].ToString() == "string | password")
                    {
                        existingField.Value = ChallangeUtil.EncryptPassword(field.Value, User.Identity.Name + existingProvider.StorageKey);
                    }
                    db.Update(existingField);
                }

                db.SaveChanges();
                transaction.Commit();
            }
        }
        return Data();
    }
}