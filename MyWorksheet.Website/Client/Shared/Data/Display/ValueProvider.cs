using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWorksheet.Website.Client.Shared.Data.Display;

public class ValueProvider<T>
{
    public ValueProvider()
    {
        NeedsReload = true;
        BoundValues = Enumerable.Empty<T>();
    }

    public IEnumerable<T> BoundValues { get; set; }

    public IEnumerable<T> ItemsSource { get; set; }
    public Func<FilterOptions, IEnumerable<T>> ItemsProvider { get; set; }
    public Func<FilterOptions, Task<IEnumerable<T>>> AsyncItemsProvider { get; set; }

    public bool NeedsReload { get; set; }

    public Task<IEnumerable<T>> GetValues(FilterOptions filter)
    {
        if (ItemsProvider != null)
        {
            return Task.FromResult(ItemsProvider(filter));
        }

        if (ItemsSource != null)
        {
            return Task.FromResult(ItemsSource);
        }

        if (AsyncItemsProvider != null)
        {
            return AsyncItemsProvider(filter);
        }

        return Task.FromResult(Array.Empty<T>() as IEnumerable<T>);
    }
}