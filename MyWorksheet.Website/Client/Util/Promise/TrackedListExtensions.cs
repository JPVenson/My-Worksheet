using System;
using System.Linq;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

namespace MyWorksheet.Website.Client.Util.Promise;

public static class TrackedListExtensions
{
    public static void RemoveId<TValue>(this IFutureList<TValue> list, Guid id) where TValue : IEntityObject
    {
        var item = list.FirstOrDefault(e => e.GetModelIdentifier() == id);
        if (item != null)
        {
            list.Remove(item);
        }
    }
}
