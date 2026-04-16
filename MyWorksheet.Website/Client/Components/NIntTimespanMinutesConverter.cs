using System;

namespace MyWorksheet.Website.Client.Components;

public class NIntTimespanMinutesConverter : IBindingConverter<TimeSpan?, int?>
{
    public static readonly IBindingConverter<TimeSpan?, int?> Converter = new NIntTimespanMinutesConverter();

    public int? ConvertTo(TimeSpan? value, Type type)
    {
        if (!value.HasValue)
        {
            return null;
        }

        return (int)value.Value.TotalMinutes;
    }

    public TimeSpan? ConvertFrom(int? value, Type type)
    {
        return TimeSpan.FromMinutes(value ?? 0);
    }
}