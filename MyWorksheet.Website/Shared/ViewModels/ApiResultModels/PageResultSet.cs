using System;
using System.Collections.Generic;
using System.Linq;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels;

public class PageResultSet<T> : ViewModelBase
{
    private ICollection<T> _currentPageItems;
    public long CurrentPage { get; set; }
    public long MaxPage { get; set; }
    public int PageSize { get; set; }
    public long TotalItemCount { get; set; }

    public ICollection<T> CurrentPageItems
    {
        get { return _currentPageItems; }
        set { SetProperty(ref _currentPageItems, value); }
    }

    public PageResultSet<TE> As<TE>(Func<T, TE> converter)
    {
        return new PageResultSet<TE>
        {
            PageSize = PageSize,
            TotalItemCount = TotalItemCount,
            CurrentPageItems = CurrentPageItems.Select(converter).ToArray(),
            CurrentPage = CurrentPage,
            MaxPage = MaxPage
        };
    }
}