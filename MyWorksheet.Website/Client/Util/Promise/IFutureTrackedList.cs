using System.Collections.Generic;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

namespace MyWorksheet.Website.Client.Util.Promise;

public interface IFutureTrackedList<TValue> : IFutureList<TValue>
    where TValue : IEntityObject
{
    void AddRange(IEnumerable<TValue> values);
    bool NeedsLoading { get; }
    Task LoadWith(Task<ApiResult<TValue[]>> promise);

    IEnumerable<EntityState<TValue>> GetStates();
    EntityState<TValue> State(TValue value);
}