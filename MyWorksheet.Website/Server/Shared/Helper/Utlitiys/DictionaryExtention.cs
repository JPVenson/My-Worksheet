using System;
using System.Collections.Generic;
using System.Linq;

namespace MyWorksheet.Webpage.Helper.Utlitiys;

public static class DictionaryExtention
{
    public static TVal GetOrNull<TKey, TVal>(this IDictionary<TKey, TVal> source, TKey name)
    {
        if (source.ContainsKey(name))
        {
            return source[name];
        }

        return default;
    }

    public static IDictionary<string, TE> ToUniqeDictionary<T, TE>(this IEnumerable<T> source,
        Func<T, string> getKey, Func<T, TE> getValue)
    {
        var flatDict = source.Select(item => new Tuple<string, TE>(getKey(item), getValue(item))).ToList();

        var resultDict = new Dictionary<string, TE>();

        foreach (var item in flatDict.GroupBy(e => e.Item1))
        {
            resultDict.Add(item.Key, item.First().Item2);
            var index = 1;
            foreach (var tuple in item.Skip(1))
            {
                resultDict.Add(item.Key + $"({index++})", tuple.Item2);
            }
        }

        return resultDict;
    }
}