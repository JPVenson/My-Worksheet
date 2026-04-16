using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.WaiterIndicator;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;

namespace MyWorksheet.Website.Client.Util.View.List;

public class PagedList<T> : List<T>, ILazyLoadedService
    where T : class
{
    public WaiterService WaiterService { get; }

    private readonly Func<PagedList<T>, Task<ApiResult<PageResultSet<T>>>> _loader;

    public PagedList(Func<PagedList<T>, Task<ApiResult<PageResultSet<T>>>> loader, WaiterService waiterService)
    {
        WaiterService = waiterService;
        _loader = loader;
        Page = 1;
        PageSize = 25;
    }

    public int Page { get; set; }
    public int PageSize { get; set; }

    public long TotalItemsCount { get; set; }
    public int MaxPages { get; set; }
    public string Error { get; set; }

    public async ValueTask SearchAsync()
    {
        var enumerable = await WaiterService.WhenTask(_loader(this));
        RefreshWith(enumerable);
    }

    public void RefreshWith(ApiResult<PageResultSet<T>> apiCall)
    {
        Clear();
        if (!apiCall.Success)
        {
            Error = apiCall.StatusMessage;
            OnDataLoaded();
            return;
        }

        TotalItemsCount = apiCall.Object.TotalItemCount;
        MaxPages = (int)apiCall.Object.MaxPage;
        foreach (var objectCurrentPageItem in apiCall.Object.CurrentPageItems)
        {
            Add(objectCurrentPageItem);
        }

        OnDataLoaded();
    }

    public event EventHandler DataLoaded;

    protected virtual void OnDataLoaded()
    {
        DataLoaded?.Invoke(this, EventArgs.Empty);
    }
}