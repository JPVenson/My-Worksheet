using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;

namespace MyWorksheet.Website.Client.Util.Promise;

public static class AsyncEnumerable
{
    public static IAsyncIEnumerable<TValue> AsPromise<TValue>(this Task<IEnumerable<TValue>> task)
    {
        return new AsyncIEnumerable<TValue>(task);
    }

    public static IAsyncIEnumerable<TValue> AsPromise<TValue>(this IEnumerable<TValue> list)
    {
        return new AsyncIEnumerable<TValue>(list);
    }

    public static Task<T> UnpackList<T>(this Task<ApiResult<T>> promise)
    {
        return promise.ContinueWith(t =>
        {
            return t.Result.UnpackOrThrow().Object;
        });
    }

    public static Task<IEnumerable<T>> UnpackList<T>(this Task<ApiResult<PageResultSet<T>>> promise)
    {
        return promise.ContinueWith(t =>
        {
            return t.Result.UnpackOrThrow().Object.CurrentPageItems.AsEnumerable();
        });
    }
}