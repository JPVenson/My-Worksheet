using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

namespace MyWorksheet.Website.Client.Util.Promise;

public interface IFutureList<TValue> : IFutureList, IList<TValue>
{
    void RemoveWhere(Func<TValue, bool> condition);
    Task<TValue> Find(Guid id);
}