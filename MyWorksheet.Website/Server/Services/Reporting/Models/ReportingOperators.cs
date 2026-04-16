using System;
using System.Collections.Generic;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Engine;

namespace MyWorksheet.Website.Server.Services.Reporting.Models;

public static class ReportingOperators
{
    public static ReportingOperator EqualTo { get; } = new ReportingOperator()
    {
        Value = "=",
        Display = "Equals",
    };

    public static ReportingOperator UnEqual { get; } = new ReportingOperator()
    {
        Value = "!=",
        Display = "Not Equals",
    };

    public static ReportingOperator BiggerAs { get; } = new ReportingOperator()
    {
        Value = ">",
        Display = "Bigger as"
    };

    public static ReportingOperator SmallerAs { get; } = new ReportingOperator()
    {
        Value = "<",
        Display = "Smaller as"
    };

    public static ReportingOperator Contains { get; } = new ReportingOperator()
    {
        ValueFormatter = (source) =>
        {
            return "%" + source + "%";
        },
        Value = "LIKE",
        Display = "Contains"
    };

    public static ReportingOperator StartsWith { get; } = new ReportingOperator()
    {
        ValueFormatter = (source) =>
        {
            return "%" + source + "";
        },
        Value = "LIKE",
        Display = "Starts with"
    };

    public static ReportingOperator EndsWith { get; } = new ReportingOperator()
    {
        ValueFormatter = (source) =>
        {
            return "" + source + "%";
        },
        Value = "LIKE",
        Display = "Ends with"
    };

    public static ReportingOperator NotContains { get; } = new ReportingOperator()
    {
        ValueFormatter = (source) =>
        {
            return "%" + source + "%";
        },
        Value = "NOT LIKE",
        Display = "Contains Not"
    };

    public static ReportingOperator NotStartsWith { get; } = new ReportingOperator()
    {
        ValueFormatter = (source) =>
        {
            return "%" + source + "";
        },
        Value = "NOT LIKE",
        Display = "Starts not with"
    };

    public static ReportingOperator NotEndsWith { get; } = new ReportingOperator()
    {
        ValueFormatter = (source) =>
        {
            return "" + source + "%";
        },
        Value = "NOT LIKE",
        Display = "Ends not with"
    };

    public static ReportingOperator Whildcard { get; } = new ReportingOperator()
    {
        ValueFormatter = (source) =>
        {
            var input = source.ToString();

            input = input.Replace(@"\", @"\\");
            input = input.Replace(@"%", @"\%");
            input = input.Replace(@"[", @"\[");
            input = input.Replace(@"]", @"\]");
            input = input.Replace(@"_", @"\_");

            return "" + input.ToString() + " ESCAPE '\'";
        },
        Value = "LIKE",
        Display = "Whildcard",
        HelpText = "Use '*' for a single character and '**' for multibe characters." +
                   " Escape a '*' with '/*' Without quotes."
    };

    private static Dictionary<Type, IEnumerable<ReportingOperator>> _predefinedOperators;

    public static IEnumerable<ReportingOperator> GetForType(Type type)
    {
        if (_predefinedOperators.ContainsKey(type))
        {
            return _predefinedOperators[type];
        }
        return Enumerate();
    }

    static ReportingOperators()
    {
        _predefinedOperators = new Dictionary<Type, IEnumerable<ReportingOperator>>
        {
            [typeof(string)] = new[]
        {
            EqualTo,
            UnEqual,
            Contains,
            StartsWith,
            EndsWith,
            NotContains,
            NotStartsWith,
            NotEndsWith,
            Whildcard,
        },

            [typeof(DateTimeOffset)] = new[]
        {
            EqualTo,
            UnEqual,
            Contains,
            StartsWith,
            EndsWith,
            NotContains,
            NotStartsWith,
            NotEndsWith,
            Whildcard,

            BiggerAs,
            SmallerAs
        },

            [typeof(int)] = new[]
        {
            EqualTo,
            UnEqual,
            BiggerAs,
            SmallerAs
        },

            [typeof(bool)] = new[]
        {
            EqualTo,
            UnEqual,
        }
        };
    }

    public static IEnumerable<ReportingOperator> Enumerate()
    {
        yield return EqualTo;
        yield return UnEqual;

        yield return BiggerAs;
        yield return SmallerAs;

        yield return Contains;
        yield return StartsWith;
        yield return EndsWith;

        yield return NotContains;
        yield return NotStartsWith;
        yield return NotEndsWith;

        yield return Whildcard;
    }
}