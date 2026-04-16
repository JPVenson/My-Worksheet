using System.Collections.Concurrent;

using System;
namespace MyWorksheet.Website.Server.Services.Blob.Thumbnail;

public interface ITempFileTokenService
{
    ConcurrentDictionary<string, FileToken> TokensIssued { get; set; }

    string IssueFileToken(Guid userId, Guid storageId, string callerIp);
    FileToken? RedeemToken(string token, Guid storageId, string callerIp, bool removeFromStorage);
}