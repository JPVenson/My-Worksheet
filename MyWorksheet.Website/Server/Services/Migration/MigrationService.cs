namespace MyWorksheet.Website.Server.Services.Migration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

/// <summary>
/// Handles Migration of the Jellyfin data structure.
/// </summary>
public class MigrationService
{
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;
    private readonly ILogger<MigrationService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MigrationService"/> class.
    /// </summary>
    /// <param name="dbContextFactory">Provides access to the jellyfin database.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="startupLogger">The startup logger for Startup UI intigration.</param>
    /// <param name="applicationPaths">Application paths for library.db backup.</param>
    /// <param name="backupService">The jellyfin backup service.</param>
    /// <param name="jellyfinDatabaseProvider">The jellyfin database provider.</param>
    public MigrationService(
        IDbContextFactory<MyworksheetContext> dbContextFactory,
        ILogger<MigrationService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
#pragma warning disable CS0618 // Type or member is obsolete
        Migrations = [.. typeof(IAsyncMigrationRoutine).Assembly.GetTypes().Where(e => typeof(IAsyncMigrationRoutine).IsAssignableFrom(e))
            .Select(e => (Type: e, Metadata: e.GetCustomAttribute<MigrationAttribute>()))
            .Where(e => e.Metadata != null)
            .GroupBy(e => e.Metadata!.Stage)
            .Select(f =>
            {
                var stage = new MigrationStage(f.Key);
                foreach (var item in f)
                {
                    stage.Add(new(item.Type, item.Metadata!));
                }

                return stage;
            })];
#pragma warning restore CS0618 // Type or member is obsolete
    }

    private interface IInternalMigration
    {
        Task PerformAsync(ILogger logger);
    }

    private HashSet<MigrationStage> Migrations { get; set; }

    public async Task MigrateStepAsync(MigrationStageTypes stage, IServiceProvider? serviceProvider)
    {
        ICollection<CodeMigration> migrationStage = (Migrations.FirstOrDefault(e => e.Stage == stage) as ICollection<CodeMigration>) ?? [];

        var dbContext = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
        await using (dbContext.ConfigureAwait(false))
        {
            var historyRepository = dbContext.GetService<IHistoryRepository>();
            var migrationsAssembly = dbContext.GetService<IMigrationsAssembly>();
            var appliedMigrations = await historyRepository.GetAppliedMigrationsAsync().ConfigureAwait(false);
            var pendingCodeMigrations = migrationStage
                .Where(e => appliedMigrations.All(f => f.MigrationId != e.BuildCodeMigrationId()))
                .Select(e => (Key: e.BuildCodeMigrationId(), Migration: new InternalCodeMigration(e, serviceProvider, dbContext)))
                .ToArray();

            (string Key, InternalDatabaseMigration Migration)[] pendingDatabaseMigrations = [];
            if (stage is MigrationStageTypes.CoreInitialisation)
            {
                pendingDatabaseMigrations = migrationsAssembly.Migrations.Where(f => appliedMigrations.All(e => e.MigrationId != f.Key))
                   .Select(e => (Key: e.Key, Migration: new InternalDatabaseMigration(e, dbContext)))
                   .ToArray();
            }

            (string Key, IInternalMigration Migration)[] pendingMigrations = [.. pendingCodeMigrations, .. pendingDatabaseMigrations];
            _logger.LogInformation("There are {Pending} migrations for stage {Stage}.", pendingCodeMigrations.Length, stage);
            var migrations = pendingMigrations.OrderBy(e => e.Key).ToArray();

            foreach (var item in migrations)
            {
                var migrationLogger = _logger;
                try
                {
                    migrationLogger.LogInformation("Perform migration {Name}", item.Key);
                    await item.Migration.PerformAsync(migrationLogger).ConfigureAwait(false);
                    migrationLogger.LogInformation("Migration {Name} was successfully applied", item.Key);
                }
                catch (Exception ex)
                {
                    migrationLogger.LogCritical("Error: {Error}", ex.Message);
                    migrationLogger.LogError(ex, "Migration {Name} failed", item.Key);

                    throw;
                }
            }
        }
    }

    private static string GetJellyfinVersion()
    {
        return Assembly.GetEntryAssembly()!.GetName().Version!.ToString();
    }

    private class InternalCodeMigration : IInternalMigration
    {
        private readonly CodeMigration _codeMigration;
        private readonly IServiceProvider? _serviceProvider;
        private MyworksheetContext _dbContext;

        public InternalCodeMigration(CodeMigration codeMigration, IServiceProvider? serviceProvider, MyworksheetContext dbContext)
        {
            _codeMigration = codeMigration;
            _serviceProvider = serviceProvider;
            _dbContext = dbContext;
        }

        public async Task PerformAsync(ILogger logger)
        {
            await _codeMigration.Perform(_serviceProvider, logger, CancellationToken.None).ConfigureAwait(false);

            var historyRepository = _dbContext.GetService<IHistoryRepository>();
            var createScript = historyRepository.GetInsertScript(new HistoryRow(_codeMigration.BuildCodeMigrationId(), GetJellyfinVersion()));
            await _dbContext.Database.ExecuteSqlRawAsync(createScript).ConfigureAwait(false);
        }
    }

    private class InternalDatabaseMigration : IInternalMigration
    {
        private readonly MyworksheetContext _MyworksheetContext;
        private KeyValuePair<string, TypeInfo> _databaseMigrationInfo;

        public InternalDatabaseMigration(KeyValuePair<string, TypeInfo> databaseMigrationInfo, MyworksheetContext MyworksheetContext)
        {
            _databaseMigrationInfo = databaseMigrationInfo;
            _MyworksheetContext = MyworksheetContext;
        }

        public async Task PerformAsync(ILogger logger)
        {
            var migrator = _MyworksheetContext.GetService<IMigrator>();
            await migrator.MigrateAsync(_databaseMigrationInfo.Key).ConfigureAwait(false);
        }
    }
}