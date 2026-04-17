using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyWorksheet.Webpage.Helper;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.NumberRangeService;
using MyWorksheet.Website.Server.Services.UserCounter;
using MyWorksheet.Website.Server.Settings;

namespace MyWorksheet.Website.Server.Services.Migration.Migrations.v1._00;

[Migration("2026-04-17T01:00:00", "CreateDefaultUser", RunMigrationOnSetup = true, Stage = MigrationStageTypes.AppInitialisation)]
public class CreateDefaultUserMigration : IAsyncMigrationRoutine
{
    private readonly IOptions<AppServerSettings> _serverOptions;
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;
    private readonly ILogger<CreateDefaultUserMigration> _logger;
    private readonly IUserQuotaService _userQuotaService;
    private readonly INumberRangeService _numberRangeService;

    public CreateDefaultUserMigration(
        IOptions<AppServerSettings> serverOptions,
        IDbContextFactory<MyworksheetContext> dbContextFactory,
        ILogger<CreateDefaultUserMigration> logger,
        IUserQuotaService userQuotaService,
        INumberRangeService numberRangeService)
    {
        _serverOptions = serverOptions;
        _dbContextFactory = dbContextFactory;
        _logger = logger;
        _userQuotaService = userQuotaService;
        _numberRangeService = numberRangeService;
    }

    public async Task PerformAsync(CancellationToken cancellationToken)
    {
        var defaultUserConfig = _serverOptions.Value.User.Default;

        if (defaultUserConfig is null or { Password: null, Username: null })
        {
            throw new InvalidOperationException("Default user configuration not found in settings.");
        }

        await using var db = await _dbContextFactory.CreateDbContextAsync();
        var user = AccountHelper.CreateUser(db, new()
        {
            Username = defaultUserConfig.Username,
            NeedPasswordReset = false,
            UserPlainTextPassword = defaultUserConfig.Password,
            RegionId = (await db.PromisedFeatureRegions.FirstAsync()).PromisedFeatureRegionId
        }, AccountHelper.CreateDefaultAddress(),
            _serverOptions.Value.User.Create.DefaultRoles,
            _logger,
            _userQuotaService,
            _numberRangeService);
    }
}