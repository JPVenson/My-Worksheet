using System;
using System.Linq;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Blob;
using MyWorksheet.Website.Server.Services.Blob.Thumbnail;
using MyWorksheet.Website.Server.Settings;
using MyWorksheet.Website.Server.Shared.TaskScheduling;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MyWorksheet.Website.Server.Services.ScheduledTasks;

[ScheduleTaskEvery(0, 30, 0)]
public class CleanupTempFileTokens : BaseTask
{
    private readonly ITempFileTokenService _tempFileTokenService;
    private readonly IOptions<AppServerSettings> _serverSettings;

    public CleanupTempFileTokens(ITempFileTokenService tempFileTokenService,
        IOptions<AppServerSettings> serverSettings, IDbContextFactory<MyworksheetContext> dbContextFactory) : base(dbContextFactory)
    {
        _tempFileTokenService = tempFileTokenService;
        _serverSettings = serverSettings;
    }

    public override void DoWork(TaskContext context)
    {
        var tokenTtl = _serverSettings.Value.Storage.File.Token.MaxTtl;
        var expiredTokens =
            _tempFileTokenService.TokensIssued.Where(e => e.Value.Issued + TimeSpan.FromSeconds(tokenTtl) < DateTime.UtcNow)
                .ToArray();
        context.Logger.LogVerbose($"Found {expiredTokens.Length} expired tokens in store");
        foreach (var expiredToken in expiredTokens)
        {
            FileToken x;
            if (!_tempFileTokenService.TokensIssued.TryRemove(expiredToken.Key, out x))
            {
                context.Logger.LogWarning("Could not delete token" + expiredToken.Value);
            }
        }
    }

    public override string NamedTask { get; protected set; } = "Cleanup TempFileTokens";
}