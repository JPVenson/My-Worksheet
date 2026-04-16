using System;
using System.Collections.Generic;
using System.Linq;

namespace MyWorksheet.Website.Server.Helper;

public static class PaginationHelper
{
    public static IDataPager<TEntity> ForPagedResult<TEntity>(this IQueryable<TEntity> query, int page, int pageSize)
    {
        var maxItems = query.Count();
        return new EFCoreDataPager<TEntity>()
        {
            CurrentPage = page,
            PageSize = pageSize,
            MaxPage = (int)Math.Ceiling((double)(maxItems / pageSize)),
            TotalItemCount = maxItems,
            CurrentPageItems = query.ToArray(),
        };
    }

    private class EFCoreDataPager<TEntity> : IDataPager<TEntity>
    {
        public int CurrentPage { get; set; }

        public int MaxPage { get; set; }

        public int PageSize { get; set; }

        public long TotalItemCount { get; set; }

        public ICollection<TEntity> CurrentPageItems { get; set; }
    }
}

public interface IDataPager<TEntity>
{
    public int CurrentPage { get; set; }

    public int MaxPage { get; set; }

    public int PageSize { get; set; }

    public long TotalItemCount { get; set; }

    public ICollection<TEntity> CurrentPageItems { get; set; }
}