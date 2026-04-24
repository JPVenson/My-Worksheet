using System;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Shared.Hubs.Hubs;
using MyWorksheet.Website.Shared.ViewModels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Services.ObjectChanged;

[SingletonService()]
public class ObjectChangedService
{
    private readonly IHubContext<ObjectChangedHub, IObjectChangedHub> _objectChangedHub;
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;

    public ObjectChangedService(
            IHubContext<ObjectChangedHub, IObjectChangedHub> objectChangedHub,
            IDbContextFactory<MyworksheetContext> dbContextFactory)
    {
        _objectChangedHub = objectChangedHub;
        _dbContextFactory = dbContextFactory;
    }

    public Task SendObjectChanged(ChangeEventTypes changeType, string type, Guid id, string calleeId, Guid? userId = null)
    {
        return SendObjectChanged(changeType, type, new Guid[] { id }, calleeId, userId);
    }

    public Task SendObjectChanged(ChangeEventTypes changeType, string type, Guid[] id, string calleeId,
            Guid? userId = null)
    {
        var group = "ALL";
        if (userId.HasValue && userId.HasValue)
        {
            group = "ChangeTracking_" + userId.Value;
        }

        if (calleeId != null)
        {
            return _objectChangedHub
                    .Clients
                    .GroupExcept(group, calleeId)
                    .ObjectChanged(type, id, changeType, userId);
        }

        return _objectChangedHub
                .Clients
                .Group(group)
                .ObjectChanged(type, id, changeType, userId);
    }

    public Task SendObjectChanged(ChangeEventTypes changeType, Type value, string calleeId, Guid? userId = null)
    {
        return SendObjectChanged(changeType, value.Name, calleeId, userId);
    }

    public Task SendObjectChanged(ChangeEventTypes changeType, Type value, Guid[] id, string calleeId, Guid? userId = null)
    {
        return SendObjectChanged(changeType, value.Name, id, calleeId, userId);
    }

    public Task SendObjectChanged(ChangeEventTypes changeType, Type value, Guid id, string calleeId, Guid? userId = null)
    {
        return SendObjectChanged(changeType, value.Name, id, calleeId, userId);
    }

    public Task SendObjectChanged<T>(ChangeEventTypes changeType, T value, string calleeId, Guid? userId = null)
    {
        return SendObjectChanged(changeType, new[] { value }, calleeId, userId);
    }

    public Task SendObjectChanged<T>(ChangeEventTypes changeType, T[] value, string calleeId, Guid? userId = null)
    {
        var classInfo = typeof(T);

        if (userId == null)
        {
            var creatorProperty = classInfo
                    .GetProperties()
                    .FirstOrDefault(e => e.PropertyType == typeof(AppUser));
            if (creatorProperty == null)
            {
                throw new InvalidOperationException("Cannot determine the AppUser for the object " + value);
            }

            foreach (var item in value)
            {
                var usrId = (Guid?)creatorProperty.GetValue(item);
                if (userId != null && usrId != userId)
                {
                    throw new InvalidOperationException(
                            $"Cannot send a message containing an object that does not belong to that user '{typeof(T)}'");
                }

                userId = usrId;
            }
        }
        using var db = _dbContextFactory.CreateDbContext();
        var entityType = db.Model.FindEntityType(typeof(T));
        var primaryKeyInfo = entityType.FindPrimaryKey();
        var primaryKeyProperty = classInfo.GetProperties()
                .First(e => e.Name == primaryKeyInfo.Properties[0].Name);

        var ids = new Guid[value.Length];
        for (int i = 0; i < value.Length; i++)
        {
            Guid primaryKeyValue = Guid.Empty;
            var primaryKey = primaryKeyProperty.GetValue(value[i]);
            if (primaryKey is Guid guidVal)
            {
                primaryKeyValue = guidVal;
            }

            ids[i] = primaryKeyValue;
        }

        return SendObjectChanged(changeType, typeof(T).Name, ids, calleeId, userId);
    }
}
