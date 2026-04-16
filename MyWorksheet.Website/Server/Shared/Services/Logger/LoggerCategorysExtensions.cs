using System;
using System.Collections.Generic;
using System.Linq;

namespace MyWorksheet.Website.Server.Shared.Services.Logger;

public static class LoggerCategorysExtensions
{
    public static string ToString(this LoggerCategories category)
    {
        return category.MaskToList<LoggerCategories>().Select(e => e.ToString()).Aggregate((e, f) => e + "&" + f);
    }

    public static IEnumerable<T> MaskToList<T>(this Enum mask)
    {
        if (typeof(T).IsSubclassOf(typeof(Enum)) == false)
        {
            throw new ArgumentException();
        }

        return Enum.GetValues(typeof(T))
            .Cast<Enum>()
            .Where(mask.HasFlag)
            .Cast<T>();
    }

    public static bool HasFlagFast(this LoggerCategories value, LoggerCategories flag)
    {
        return (value & flag) != 0;
    }
}
