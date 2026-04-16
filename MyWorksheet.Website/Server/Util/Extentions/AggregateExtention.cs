using System;
using System.Collections.Generic;
using System.Linq;

namespace Katana.CommonTasks.Extentions;

public static class AggregateExtention
{
    public static TE AggregateIf<TE>(this IEnumerable<TE> value, Func<IEnumerable<TE>, bool> condition, Func<TE, TE, TE> aggregation)
    {
        if (condition(value))
        {
            return value.Aggregate(aggregation);
        }
        return default(TE);
    }
}