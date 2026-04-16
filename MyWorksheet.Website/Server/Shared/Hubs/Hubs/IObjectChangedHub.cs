using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyWorksheet.Website.Shared.ViewModels;

namespace MyWorksheet.Website.Server.Shared.Hubs.Hubs;

public interface IObjectChangedHub
{
    Task ObjectChanged<T>(string typeName, T value, ChangeEventTypes changeEventTypes, Guid? userId);
    Task ObjectChanged(string typeName, Guid? id);
    Task ObjectChanged(string typeName, Guid id, byte[] lastState, IDictionary<string, object> changes);
}
