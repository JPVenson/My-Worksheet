using System;

namespace MyWorksheet.Website.Client.Components;

public class IntTimespanMinutesConverter : IBindingConverter<TimeSpan?, int>
{
    public static readonly IBindingConverter<TimeSpan?, int> Converter = new IntTimespanMinutesConverter();

    public int ConvertTo(TimeSpan? value, Type type)
    {
        return (int)value.Value.TotalMinutes;
    }

    public TimeSpan? ConvertFrom(int value, Type type)
    {
        return TimeSpan.FromMinutes(value);
    }
}