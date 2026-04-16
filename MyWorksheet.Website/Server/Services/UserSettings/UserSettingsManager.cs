using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MyWorksheet.Public.Models.Attr;
using MyWorksheet.Webpage.Helper;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.UserSettings;
using Microsoft.EntityFrameworkCore;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Services.UserSettings;

[SingletonService(typeof(IUserSettingsManager))]
public class UserSettingsManager : IUserSettingsManager
{
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;

    public UserSettingsManager(IDbContextFactory<MyworksheetContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;

        SettingsTypes = typeof(WorksheetUiOptions).Assembly.GetTypes()
                                                  .Where(
                                                         e => e.GetCustomAttribute(typeof(SettingsElementAttribute)) != null)
                                                  .ToDictionary(e => e.GetCustomAttribute<SettingsElementAttribute>().Name,
                                                                e => e);
    }

    public IDictionary<string, Type> SettingsTypes { get; }

    public void UpdateSetting(object setting, string name, string key, Guid userId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var listRep = setting.ToJsonElements();
        var group = GetOrCreateGroup(userId, name, key, db);

        foreach (var o in listRep)
        {
            CreateOrUpdateSetting(group.SettingsGroupId, o.Value?.ToString(), o.Key, db, userId);
        }
        db.SaveChanges();
    }

    public object GetSettingFromStore(string name, string key, Guid userId)
    {
        var type = SettingsTypes[name];

        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        using var db = _dbContextFactory.CreateDbContext();

        if (userId == Guid.Empty)
        {
            return Activator.CreateInstance(type);
        }

        var storageGroup = db.SettingsGroups.Where(e => e.IdAppUser == userId && e.Key == key && e.Name == name)
                             .FirstOrDefault();

        if (storageGroup == null)
        {
            return Activator.CreateInstance(type);
        }

        var group = GetOrCreateGroup(userId, name, key, db);

        var settings = db.SettingsValues
                         .Where(f => f.IdSettingsGroup == group.SettingsGroupId)
                         .ToArray();
        db.SaveChanges();
        return settings.ToDictionary(e => e.Name, e => e.Value)
                       .FromJsonElements(type);
    }

    private SettingsGroup GetOrCreateGroup(Guid userId, string name, string key, MyworksheetContext db)
    {
        var storageGroup = db.SettingsGroups.Where(e => e.IdAppUser == userId && e.Key == key && e.Name == name)
                             .FirstOrDefault();

        if (storageGroup == null)
        {
            storageGroup = new SettingsGroup
            {
                IdAppUser = userId,
                Name = name,
                Key = key
            };
            db.Add(storageGroup);
        }

        return storageGroup;
    }

    private void CreateOrUpdateSetting(Guid groupId, string value, string name, MyworksheetContext db, Guid idCreator)
    {
        var storageValue = db.SettingsValues
                             .Where(f => f.IdSettingsGroup == groupId)
                             .Where(f => f.Name == name)
                             .FirstOrDefault();

        if (storageValue == null)
        {
            if (value == null)
            {
                return;
            }

            db.Add(new SettingsValue
            {
                Value = value,
                Name = name,
                IdSettingsGroup = groupId,
                IdAppUser = idCreator
            });
        }
        else
        {
            if (value == null)
            {
                db.SettingsValues.Where(e => e.IdSettingsGroup == groupId && e.Name == name && e.IdAppUser == idCreator).ExecuteDelete();
            }
            else
            {
                db.SettingsValues.Where(e => e.SettingsValueId == storageValue.SettingsValueId && e.IdAppUser == idCreator)
                    .ExecuteUpdate(e => e.SetProperty(f => f.Value, value));
            }
        }
    }
}
