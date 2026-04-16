using System.Collections.Generic;
using System.Linq;

namespace Katana.CommonTasks.Extentions;

public static class EnumerableExtensions
{
    public static IEnumerable<List<T>> Partition<T>(this IEnumerable<T> source, int max)
    {
        List<T> toReturn = new List<T>(max);
        foreach (var item in source)
        {
            toReturn.Add(item);
            if (toReturn.Count == max)
            {
                yield return toReturn;
                toReturn = new List<T>(max);
            }
        }
        if (toReturn.Any())
        {
            yield return toReturn;
        }
    }
}