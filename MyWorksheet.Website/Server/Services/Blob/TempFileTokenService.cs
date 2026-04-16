using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyWorksheet.Website.Server.Services.Blob.Thumbnail;
using MyWorksheet.Website.Server.Settings;
using MyWorksheet.Website.Server.Shared.Services.Logger;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using MyWorksheet.Website.Server.Util.Auth;
using MyWorksheet.Website.Shared.Util;
using Newtonsoft.Json;
using ServiceLocator.Attributes;
using Microsoft.Extensions.Options;

namespace MyWorksheet.Website.Server.Services.Blob;

[SingletonService(typeof(ITempFileTokenService))]
public class TempFileTokenService : ITempFileTokenService
{
    public TempFileTokenService(IOptions<AppServerSettings> serverSettings, IAppLogger appLogger)
    {
        _serverSettings = serverSettings;
        _appLogger = appLogger;
        TokensIssued = new ConcurrentDictionary<string, FileToken>();
    }

    private IOptions<AppServerSettings> _serverSettings;
    private readonly IAppLogger _appLogger;
    public ConcurrentDictionary<string, FileToken> TokensIssued { get; set; }

    private string EncryptToken(FileToken token)
    {
        var serializeObject = JsonConvert.SerializeObject(token);
        TokensIssued.TryAdd(serializeObject, token);
        var salt = Encoding.UTF8.GetString(ChallangeUtil.HashPassword(token.CallerIp, token.CallerIp));
        var encryptedToken = ChallangeUtil.EncryptPassword(serializeObject, salt);
        return Encoding.UTF8.GetBytes(encryptedToken).ToDecHex();
    }

    public string IssueFileToken(Guid userId, Guid storageId, string callerIp)
    {
        var tokenTtl = _serverSettings.Value.Storage.File.Token.MaxTtl;

        var hasValidToken = TokensIssued.FirstOrDefault(e => e.Value.IsValid(callerIp, storageId.ToString(), tokenTtl));
        if (hasValidToken.Key != null)
        {
            return EncryptToken(hasValidToken.Value);
        }

        return EncryptToken(new FileToken(userId, storageId, callerIp, DateTime.UtcNow));
    }

    public FileToken? RedeemToken(string token, Guid storageId, string callerIp, bool removeFromStorage)
    {
        if (token == null)
        {
            return null;
        }

        try
        {
            var reencoded = ChallangeUtil.StringToByteArrayFastest(token);

            var salt = Encoding.UTF8.GetString(ChallangeUtil.HashPassword(callerIp, callerIp));
            var realToken = ChallangeUtil.DecryptPassword(Encoding.UTF8.GetString(reencoded), salt);

            var fileToken = JsonConvert.DeserializeObject<FileToken>(realToken);

            //FileToken tokenInfo;
            //if (!TokensIssued.TryGetValue(realToken, out tokenInfo))
            //{
            //	return null;
            //}

            var validToken = fileToken.IsValid(callerIp, storageId.ToString(), _serverSettings.Value.Storage.File.Token.MaxTtl);
            if (!validToken)
            {
                return null;
            }
            return fileToken;
        }
        catch (Exception exception)
        {
            _appLogger.LogWarning("RedeemToken failed", LoggerCategories.FileUpload.ToString(),
                new Dictionary<string, string>()
                {
                    {nameof(storageId), storageId.ToString()},
                    {nameof(callerIp), callerIp.ToString()},
                    {nameof(exception), JsonConvert.SerializeObject(exception)},
                });
            return null;
        }
    }
}