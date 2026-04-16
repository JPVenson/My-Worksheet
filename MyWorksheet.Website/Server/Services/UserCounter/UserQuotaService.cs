using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.ObjectChanged;
using MyWorksheet.Website.Server.Settings;
using MyWorksheet.Website.Server.Shared.Services.Logger;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using MyWorksheet.Website.Shared.ViewModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Services.UserCounter;

[SingletonService(typeof(IUserQuotaService))]
public class UserQuotaService : IUserQuotaService
{
    private readonly IAppLogger _appLogger;
    private readonly ObjectChangedService _objectChangedService;
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;
    private readonly IOptions<AccountDefaultQuotaSettings> _quotaSettings;

    public UserQuotaService(IOptions<AccountDefaultQuotaSettings> quotaSettings,
        IDbContextFactory<MyworksheetContext> dbContextFactory,
        IAppLogger appLogger,
        ObjectChangedService objectChangedService)
    {
        _quotaSettings = quotaSettings;
        _dbContextFactory = dbContextFactory;
        _appLogger = appLogger;
        _objectChangedService = objectChangedService;
        UserQuotaParts = new Dictionary<Quotas, UserQuotaPart>
        {
            { Quotas.Project, new UserQuotaPart("Project", (int)Quotas.Project) },
            { Quotas.Worksheet, new UserQuotaPart("Worksheet", (int)Quotas.Worksheet) },
            { Quotas.Webhooks, new UserQuotaPart("Webhocks", (int)Quotas.Webhooks) },
            { Quotas.LocalFile, new UserQuotaPart("LocalFile", (int)Quotas.LocalFile) },
            {
                Quotas.ConurrentReports,
                new UserQuotaPart("ConurrentReports", (int)Quotas.ConurrentReports)
            }
        };
    }

    public IDictionary<Quotas, UserQuotaPart> UserQuotaParts { get; set; }

    public void CreateDefaultQuotas(Guid userId, MyworksheetContext db)
    {
        foreach (var enumValue in UserQuotaParts)
        {
            var scotaMax = GetDefaultQuota(enumValue.Key);

            var hasQuota = db.UserQuota
                .Where(f => f.IdAppUser == userId)
                .Where(f => f.QuotaType == enumValue.Value.TypeKey)
                .FirstOrDefault();

            if (hasQuota == null)
            {
                db.Add(new UserQuota
                {
                    IdAppUser = userId,
                    QuotaType = enumValue.Value.TypeKey,
                    QuotaMin = 0,
                    QuotaMax = scotaMax,
                    QuotaUnlimited = scotaMax == -1,
                    QuotaValue = 0
                });
            }
        }
    }

    public bool Allowed(Guid userId, Quotas quota)
    {
        return true;
    }

    private int GetDefaultQuota(Quotas quota)
    {
        var settings = _quotaSettings.Value;
        return quota switch
        {
            Quotas.Project => settings.Project,
            Quotas.Worksheet => settings.Worksheet,
            Quotas.Webhooks => settings.Webhooks,
            Quotas.LocalFile => settings.LocalFile,
            Quotas.ConurrentReports => settings.ConcurrentReports,
            _ => 0
        };
    }

    public async Task Expand(Guid userId, long value, Quotas quota)
    {
        using var db = _dbContextFactory.CreateDbContext();
        db.UserQuota.Where(f => f.IdAppUser == userId)
            .Where(f => f.QuotaType == (int)quota)
            .ExecuteUpdate(f => f.SetProperty(e => e.QuotaMax, e => e.QuotaMax + value));

        await _objectChangedService.SendObjectChanged(ChangeEventTypes.Changed, typeof(UserQuota), Guid.Empty, null, userId);
    }

    public async Task Reduce(Guid userId, long value, Quotas quota)
    {
        using var db = _dbContextFactory.CreateDbContext();

        db.UserQuota.Where(f => f.IdAppUser == userId)
            .Where(f => f.QuotaType == (int)quota)
            .ExecuteUpdate(f => f.SetProperty(e => e.QuotaMax, e => e.QuotaMax - value));
        await _objectChangedService.SendObjectChanged(ChangeEventTypes.Changed, typeof(UserQuota), Guid.Empty, null, userId);
    }

    public async Task<bool> Add(Guid userId, long value, Quotas quota)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var userQuota = db.UserQuota
            .Where(f => f.IdAppUser == userId)
            .Where(f => f.QuotaType == (int)quota)
            .First();
        if (!userQuota.QuotaUnlimited && userQuota.QuotaValue + value >= userQuota.QuotaMax)
        {
            return false;
        }

        db.UserQuota.Where(f => f.IdAppUser == userId)
            .Where(f => f.QuotaType == (int)quota)
            .ExecuteUpdate(f => f.SetProperty(e => e.QuotaValue, e => e.QuotaValue + value));
        await _objectChangedService.SendObjectChanged(ChangeEventTypes.Changed, userQuota, null, userId);
        return true;
    }

    public async Task<bool> Subtract(Guid userId, long value, Quotas quota)
    {
        using var db = _dbContextFactory.CreateDbContext();

        db.UserQuota.Where(f => f.IdAppUser == userId)
            .Where(f => f.QuotaType == (int)quota)
            .Where(f => f.QuotaUnlimited == false)
            .ExecuteUpdate(f => f.SetProperty(e => e.QuotaValue, e => e.QuotaValue - value));

        var entry = db.UserQuota
            .Where(f => f.IdAppUser == userId)
            .Where(f => f.QuotaType == (int)quota)
            .FirstOrDefault();
        await _objectChangedService.SendObjectChanged(ChangeEventTypes.Changed, entry, null, userId);

        if (entry != null && entry.QuotaValue < 0)
        {
            _appLogger
                .LogError("A user has created an invalid Quota value", LoggerCategories.Throttle.ToString(),
                    new Dictionary<string, string>
                    {
                        {"Entry", JsonConvert.SerializeObject(entry)}
                    });
        }

        return true;
    }
}