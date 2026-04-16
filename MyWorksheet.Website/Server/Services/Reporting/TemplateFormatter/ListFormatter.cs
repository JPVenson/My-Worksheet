using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using System.Linq.Dynamic.Core;
using MyWorksheet.Website.Server.Models;
using Morestachio.Formatter.Framework;
using Morestachio.Formatter.Framework.Attributes;

namespace MyWorksheet.Website.Server.Services.Reporting.TemplateFormatter;

public static class ListFormatter
{
    //[MorestachioFormatter("group by week", "Groups the WorksheetItems by Weeks")]
    //public static IEnumerable<IGrouping<int, WorksheetItemModel>> FirstOrDefault(IEnumerable<WorksheetItemModel> sourceCollection, string arguments)
    //{
    //	return sourceCollection.GroupBy(e => GlobalFormatter.WeekOfDate(e.DateOfAction));
    //}

    [MorestachioFormatter("GroupByWeek", "Groups the WorksheetItems by Weeks")]
    public static IEnumerable<IGrouping<int, PerDayReporting>> FirstOrDefault(IEnumerable<PerDayReporting> sourceCollection)
    {
        return sourceCollection.GroupBy(e => GlobalFormatter.WeekOfDate(e.DateOfAction));
    }

    [MorestachioFormatter("GroupByWeek", "Groups the WorksheetItems by Weeks")]
    public static IEnumerable<IGrouping<int, WorksheetItemReporting>> FirstOrDefault(IEnumerable<WorksheetItemReporting> sourceCollection)
    {
        return sourceCollection.GroupBy(e => GlobalFormatter.WeekOfDate(e.DateOfAction));
    }

    [MorestachioFormatter("Concat", "partitions the list")]
    public static IEnumerable<T> Partition<T>(IEnumerable<T> source, [RestParameter] params object[] rest)
    {
        foreach (var o in rest)
        {
            if (o is IEnumerable<T> canConcat)
            {
                source = source.Concat(canConcat);
            }
        }

        return source;
    }

    [MorestachioFormatter("Partition", "partitions the list")]
    public static IEnumerable<List<T>> Partition<T>(IEnumerable<T> source, decimal size)
    {
        IList<T> target;
        if (source is IList<T>)
        {
            target = source as IList<T>;
        }
        else
        {
            target = source.ToArray();
        }

        var sizeInt = size;

        for (var i = 0; i < Math.Ceiling(target.Count / (double)sizeInt); i++)
        {
            yield return new List<T>(target.Skip((int)(sizeInt * i)).Take((int)sizeInt));
        }
    }

    [MorestachioFormatter("ListMax", "Called on a list of numbers it returns the biggest")]
    public static T ListMax<T>(IEnumerable<T> sourceCollection)
    {
        return sourceCollection.Max();
    }

    [MorestachioFormatter("ListMin", "Called on a list of numbers it returns the smallest")]
    public static T ListMin<T>(IEnumerable<T> sourceCollection)
    {
        return sourceCollection.Min();
    }

    [MorestachioFormatter("Contains", "Searches in the list for that the argument")]
    [MorestachioFormatterInput("Must be ether a fixed value or an reference $other$")]
    public static bool Contains<T>(IEnumerable<T> sourceCollection, object arguments)
    {
        return sourceCollection.Any(e => e.Equals(arguments));
    }

    [MorestachioFormatter("ElementAt", "Gets the item in the list on the position")]
    [MorestachioFormatterInput("Must be a number")]
    public static T ElementAt<T>(IEnumerable<T> sourceCollection, string arguments)
    {
        return sourceCollection.ElementAtOrDefault(int.Parse(arguments));
    }

    [MorestachioFormatter("Distinct", "Gets a new list that contains not duplicates")]
    public static IEnumerable<T> Distinct<T>(IEnumerable<T> sourceCollection)
    {
        return sourceCollection.Distinct();
    }

    [MorestachioFormatter("FlatGroup", "Flattens the Group returned by group by", ReturnHint = "Can be listed with #each")]
    [MorestachioFormatterInput("Must be Expression to property")]
    public static IEnumerable<T> GroupByList<TKey, T>(IGrouping<TKey, T> sourceCollection)
    {
        return sourceCollection.ToList();
    }

    [MorestachioFormatter("Aggregate", "Aggreates the elements and returns it")]
    public static object Aggregate(IEnumerable sourceCollection)
    {
        return Sum(sourceCollection);
    }

    [MorestachioFormatter("Sum", "Aggreates the elements and returns it")]
    public static object Sum(IEnumerable sourceCollection)
    {
        var colQuery = sourceCollection.AsQueryable();

        if (typeof(int).IsAssignableFrom(colQuery.ElementType))
        {
            return colQuery.Cast<int>().Sum();
        }
        else if (typeof(long).IsAssignableFrom(colQuery.ElementType))
        {
            return colQuery.Cast<long>().Sum();
        }
        else if (typeof(decimal).IsAssignableFrom(colQuery.ElementType))
        {
            return colQuery.Cast<decimal>().Sum();
        }
        else if (typeof(double).IsAssignableFrom(colQuery.ElementType))
        {
            return colQuery.Cast<double>().Sum();
        }
        else if (typeof(float).IsAssignableFrom(colQuery.ElementType))
        {
            return colQuery.Cast<float>().Sum();
        }

        if (Nullable.GetUnderlyingType(colQuery.ElementType) != null)
        {
            if (typeof(int?).IsAssignableFrom(colQuery.ElementType))
            {
                return colQuery.Cast<int?>().Sum();
            }
            else if (typeof(long?).IsAssignableFrom(colQuery.ElementType))
            {
                return colQuery.Cast<long?>().Sum();
            }
            else if (typeof(decimal?).IsAssignableFrom(colQuery.ElementType))
            {
                return colQuery.Cast<decimal?>().Sum();
            }
            else if (typeof(double?).IsAssignableFrom(colQuery.ElementType))
            {
                return colQuery.Cast<double?>().Sum();
            }
            else if (typeof(float?).IsAssignableFrom(colQuery.ElementType))
            {
                return colQuery.Cast<float?>().Sum();
            }
        }

        return FormatterFlow.Skip;
    }

    [MorestachioFormatter("Sum", "Aggreates the property in the argument and returns it")]
    public static int Sum(IEnumerable<int> sourceCollection)
    {
        return sourceCollection.Sum();
    }

    [MorestachioFormatter("Sum", "Aggreates the property in the argument and returns it")]
    public static decimal Sum(IEnumerable<decimal> sourceCollection)
    {
        return sourceCollection.Sum();
    }

    [MorestachioFormatter("Sum", "Aggreates the property in the argument and returns it")]
    public static double Sum(IEnumerable<double> sourceCollection)
    {
        return sourceCollection.Sum();
    }

    [MorestachioFormatter("Last", "returns the last item in the collection")]
    public static T Last<T>(IEnumerable<T> source)
    {
        return source.LastOrDefault();
    }

    [MorestachioFormatter("First", "returns the first item in the collection")]
    public static T First<T>(IEnumerable<T> source)
    {
        return source.FirstOrDefault();
    }

    [MorestachioFormatter("FirstNotNull", "returns the first item in the collection")]
    public static T FirstNotNull<T>(IEnumerable<T> source)
    {
        return source.Where(e => e != null).FirstOrDefault();
    }
}