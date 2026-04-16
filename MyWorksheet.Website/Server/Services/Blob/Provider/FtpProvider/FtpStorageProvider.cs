using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Security;
using System.Threading.Tasks;
using MyWorksheet.Helper;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Public.Models.ObjectSchema;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.ExternalDomainValidator;
using MyWorksheet.Website.Server.Services.StreamPool;
using MyWorksheet.Website.Server.Services.Text;
using MyWorksheet.Website.Server.Shared.ObjectSchema;
using MyWorksheet.Website.Shared.Util;
using MyWorksheet.Website.Shared.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Services.Blob.Provider.FtpProvider;

public class FtpStorageProvider : BlobProviderBase
{
    private readonly IExternalDomainValidator _externalDomainValidator;
    private readonly ILocalFileStreamPoolService _fileStreamPoolService;
    private readonly ITextService _textService;

    public FtpStorageProvider(Guid storageInstance,
        StorageProviderData[] data,
        IDbContextFactory<MyworksheetContext> dbContextFactory,
        IExternalDomainValidator externalDomainValidator,
        ILocalFileStreamPoolService fileStreamPoolService,
        ITextService textService)
        : base(storageInstance, data, dbContextFactory)
    {
        _externalDomainValidator = externalDomainValidator;
        _fileStreamPoolService = fileStreamPoolService;
        _textService = textService;
    }

    public override async Task<IEnumerable<string>> Test(Guid appUserId)
    {
        var log = new List<string>();
        var blacklist = await EnsureDomainIsNotBlacklisted(appUserId, GetDataFromStore("Url"), "ftp");
        if (blacklist != null)
        {
            foreach (var serverProvidedTranslation in blacklist)
            {
                log.Add(_textService.Compile(serverProvidedTranslation.Key).ToString());
            }
        }
        var host = new Uri(GetDataFromStore("Url")).Host;

        if (!await _externalDomainValidator.TryCallDomain(host, appUserId))
        {
            return new[] { "DNS: Domain is blacklisted or you have tried to call it too often" };
        }

        log.AddRange(await base.Test(appUserId));
        return log;
    }

    public async Task<ServerProvidedTranslation[]> EnsureDomainIsNotBlacklisted(Guid appUserId, string url, params string[] expectedSchemes)
    {
        var urlCheck = _externalDomainValidator.ValidateUrl(url);

        if (!urlCheck.IsValid)
        {
            await Task.CompletedTask;
            return urlCheck.Error;
        }

        return null;
    }

    public override long MaxSizeInBytes { get; protected set; } = long.MaxValue;

    private async Task<FtpWebRequest> CreateDefaultRequest(string name, Guid userId)
    {
        if (await EnsureDomainIsNotBlacklisted(userId, GetDataFromStore("Url"), "ftp") != null)
        {
            throw new InvalidOperationException("Path is invalid");
        }

        var db = DbContextFactory.CreateDbContext();
        var url = new Uri(Path.Combine(GetDataFromStore("Url"), name + ".bin"));

        var request = (FtpWebRequest)WebRequest.Create(url);
        request.Timeout = (int)Timeout.TotalMilliseconds;
        var username = db.AppUsers.Find(userId).Username;
        var provider = db.StorageProviders.Find(base.StorageInstance).StorageKey;
        var password = ChallangeUtil.DecryptPassword(GetDataFromStore("Password"), username + provider);
        request.Credentials = new NetworkCredential(GetDataFromStore("Username"), password);
        request.KeepAlive = false;
        request.UseBinary = true;
        request.UsePassive = true;
        return request;
    }

    public override async Task<BlobProviderGetResult> GetData(Guid id, Guid appUserId, MyworksheetContext db)
    {
        var storageEntry = db.StorageEntries.Find(id);
        FileStream tempFile = null;
        try
        {
            var request = await CreateDefaultRequest(storageEntry.StorageKey, appUserId);
            request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            using (var webResponse = await request.GetResponseAsync())
            {
                using (var resourceData = webResponse.GetResponseStream())
                {
                    if (resourceData.CanSeek)
                    {
                        resourceData.Seek(0, SeekOrigin.Begin);
                    }
                    tempFile = _fileStreamPoolService
                        .GetLocalStream(LocalFileStreamPoolPoolService.OPKEY_STORAGE_PROVIDER, appUserId, 0)
                        .CreateTempStream();
                    await resourceData.CopyToAsync(tempFile);
                }
            }
            tempFile.Seek(0, SeekOrigin.Begin);
            return new BlobProviderGetResult(tempFile);
        }
        catch (WebException ex)
        {
            if (tempFile != null)
            {
                tempFile.Dispose();
            }
            FtpWebResponse response = (FtpWebResponse)ex.Response;
            if (response.StatusCode ==
                FtpStatusCode.ActionNotTakenFileUnavailable)
            {
                return new BlobProviderGetResult(true);
                //DeleteStorageItem(storageEntry.StorageEntryId);
            }
            else if (response.StatusCode == FtpStatusCode.AccountNeeded
                     || response.StatusCode == FtpStatusCode.NeedLoginAccount
                     || response.StatusCode == FtpStatusCode.NotLoggedIn
                     || response.StatusCode == FtpStatusCode.SendPasswordCommand
                     || response.StatusCode == FtpStatusCode.Undefined)
            {
                await _externalDomainValidator.TryCallDomain(GetDataFromStore("Url"), appUserId);
                throw;
            }
            throw;
        }
        catch (Exception)
        {
            await _externalDomainValidator.TryCallDomain(GetDataFromStore("Url"), appUserId);
            throw;
        }
    }

    public override async Task<StorageEntry> SetData(BlobData data, Guid appUserId, StorageEntityType entityType, MyworksheetContext db)
    {
        data.DataStream.Seek(0, SeekOrigin.Begin);

        var storageEntry = CreateFromBlob(db, data, entityType, appUserId);
        db.Add(storageEntry);
        try
        {
            var request = await CreateDefaultRequest(storageEntry.StorageKey, appUserId);

            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);

            request.ContentLength = data.DataStream.Length;
            using (var targetStream = request.GetRequestStream())
            {
                data.DataStream.CopyTo(targetStream);
                targetStream.Flush();
            }
        }
        catch (WebException ex)
        {
            FtpWebResponse response = (FtpWebResponse)ex.Response;
            if (response.StatusCode == FtpStatusCode.AccountNeeded
                || response.StatusCode == FtpStatusCode.NeedLoginAccount
                || response.StatusCode == FtpStatusCode.NotLoggedIn
                || response.StatusCode == FtpStatusCode.SendPasswordCommand
                || response.StatusCode == FtpStatusCode.Undefined)
            {
                await _externalDomainValidator.TryCallDomain(GetDataFromStore("Url"), appUserId);
                throw;
            }
            throw;
        }
        catch (Exception)
        {
            await _externalDomainValidator.TryCallDomain(GetDataFromStore("Url"), appUserId);
            throw;
        }

        return storageEntry;
    }

    public override async Task DeleteData(Guid id, Guid appUserId, MyworksheetContext db)
    {
        var storageEntry = db.StorageEntries.Find(id);
        try
        {
            var request = await CreateDefaultRequest(storageEntry.StorageKey, appUserId);
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);

            using (await request.GetResponseAsync())
            {
            }
        }
        catch (WebException ex)
        {
            FtpWebResponse response = (FtpWebResponse)ex.Response;
            if (response.StatusCode ==
                FtpStatusCode.ActionNotTakenFileUnavailable)
            {
                DeleteStorageItem(storageEntry.StorageEntryId);
            }
            else if (response.StatusCode == FtpStatusCode.AccountNeeded
                     || response.StatusCode == FtpStatusCode.NeedLoginAccount
                     || response.StatusCode == FtpStatusCode.NotLoggedIn
                     || response.StatusCode == FtpStatusCode.SendPasswordCommand
                     || response.StatusCode == FtpStatusCode.Undefined)
            {
                await _externalDomainValidator.TryCallDomain(GetDataFromStore("Url"), appUserId);
                throw;
            }
        }
        catch (Exception)
        {
            await _externalDomainValidator.TryCallDomain(GetDataFromStore("Url"), appUserId);
            throw;
        }
    }

    class FtpArguments : ArgumentsBase
    {
        [JsonDisplayKey("Common/Username")]
        public string AccountName { get; set; }

        [JsonDisplayKey("Common/Password")]
        [JsonComment("Storage/FTP.Arguments.Comments.Password")]
        public SecureString Password { get; set; }

        [JsonComment("Storage/FTP.Arguments.Comments.Url")]
        [JsonDisplayKey("Storage/FTP.Arguments.Names.Url")]
        public string Url { get; set; }
    }

    public override IObjectSchema GetSchema()
    {
        return JsonSchemaExtensions.JsonSchema(typeof(FtpArguments), null);
    }
}