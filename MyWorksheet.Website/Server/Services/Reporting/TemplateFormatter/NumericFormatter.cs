using System;
using Morestachio.Formatter.Framework.Attributes;

namespace MyWorksheet.Website.Server.Services.Reporting.TemplateFormatter;

public static class NumericFormatter
{
    [MorestachioFormatter("decimal", "Converts the value to a decimal that can be used in caluculations")]
    [MorestachioFormatterInput("")]
    public static decimal ToDecimal(object value)
    {
        if (value is decimal @decimal)
        {
            return @decimal;
        }

        if (decimal.TryParse(value.ToString(), out var nDecimal))
        {
            return nDecimal;
        }

        return decimal.Zero;
    }

    [MorestachioFormatter("Add", "Adds the amount of value")]
    [MorestachioFormatterInput("Must be any kind of number")]
    public static decimal Plus(object value, object addition)
    {
        return ToDecimal(value) + ToDecimal(addition);
    }

    [MorestachioFormatter("Subtract", "Substracts the amount of value")]
    [MorestachioFormatterInput("Must be any kind of number")]
    public static decimal Minus(object value, object addition)
    {
        return ToDecimal(value) - ToDecimal(addition);
    }

    [MorestachioFormatter("Divide", "Diverts the amount of value")]
    [MorestachioFormatterInput("Must be any kind of number")]
    public static decimal Divert(object value, object addition)
    {
        return ToDecimal(value) / ToDecimal(addition);
    }

    [MorestachioFormatter("Multiply", "Multiplies the amount of value")]
    [MorestachioFormatterInput("Must be any kind of number")]
    public static decimal Multiply(object value, object addition)
    {
        return ToDecimal(value) * ToDecimal(addition);
    }

    [MorestachioFormatter("Floor", "Returns the largest integer less than or equal to the specified decimal number.")]
    [MorestachioFormatterInput("Must be any kind of number")]
    public static decimal Floor(object value, object addition)
    {
        return Math.Floor(ToDecimal(value));
    }

    [MorestachioFormatter("Ceil", "Returns the smallest integral value that is greater than or equal to the specified decimal number")]
    [MorestachioFormatterInput("Must be any kind of number")]
    public static decimal Ceil(object value, object addition)
    {
        return Math.Ceiling(ToDecimal(value));
    }

    [MorestachioFormatter("Round", "Rounds a double-precision floating-point value to a specified number of fractional digits.")]
    [MorestachioFormatterInput("Must be any kind of number")]
    public static decimal Round(object value, object addition)
    {
        return Math.Round(ToDecimal(value), (int)ToDecimal(addition));
    }

    [MorestachioFormatter("Truncate", "Calculates the integral part of a specified decimal number.")]
    [MorestachioFormatterInput("Must be any kind of number")]
    public static decimal Truncate(object value)
    {
        return Math.Truncate(ToDecimal(value));
    }

    [MorestachioFormatter("Sqrt", "Returns the square root of a specified number.")]
    [MorestachioFormatterInput("Must be any kind of number")]
    public static decimal Sqrt(object value)
    {
        return Math.Truncate(ToDecimal(value));
    }

    [MorestachioFormatter("Abs", "Returns the absolute value of a number")]
    [MorestachioFormatterInput("Must be any kind of number")]
    public static decimal Abs(object value)
    {
        return Math.Abs(ToDecimal(value));
    }

    [MorestachioFormatter("Max", "Returns the larger of two decimal numbers.")]
    [MorestachioFormatterInput("Must be any kind of number")]
    public static decimal Max(object value, object other)
    {
        return Math.Max(ToDecimal(value), ToDecimal(other));
    }

    [MorestachioFormatter("Min", "Returns the smaller of two decimal numbers.")]
    [MorestachioFormatterInput("Must be any kind of number")]
    public static decimal Min(object value, object other)
    {
        return Math.Min(ToDecimal(value), ToDecimal(other));
    }
}