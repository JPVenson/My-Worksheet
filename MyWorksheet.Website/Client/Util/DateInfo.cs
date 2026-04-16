using System;
using System.Globalization;

namespace MyWorksheet.Website.Client.Util;

public struct DateInfo
{
    public DateTimeOffset Date { get; }

    public DateInfo(DateTimeOffset date)
    {
        Date = date;
        var today = DateTimeOffset.UtcNow;
        IsToday = today.Date == Date.Date;
        if (IsToday)
        {
            IsPast = false;
            IsFuture = false;
        }
        else
        {
            IsPast = today.Date > Date.Date;
            IsFuture = !IsPast;
        }

        Iso8601Week = GetIso8601WeekOfYear(date);
    }

    public bool IsToday { get; }
    public bool IsPast { get; set; }
    public bool IsFuture { get; set; }
    public int Iso8601Week { get; set; }

    private static CultureInfo GermanCulture = CultureInfo.GetCultureInfo("DE-DE");

    public static CultureInfo TargetCulture
    {
        get { return GermanCulture; }
    }

    // This presumes that weeks start with Monday.
    // Week 1 is the 1st week of the year with a Thursday in it.
    public static int GetIso8601WeekOfYear(DateTimeOffset time)
    {
        // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
        // be the same week# as whatever Thursday, Friday or Saturday are,
        // and we always get those right

        //DayOfWeek day = TextService.CurrentSystemUiCulture.Calendar.GetDayOfWeek(time);
        //if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
        //{
        //	time = time.AddDays(3);
        //}

        // Return the week of our adjusted day

        return TargetCulture
            .Calendar
            .GetWeekOfYear(time.Date, CalendarWeekRule.FirstDay, TargetCulture.DateTimeFormat.FirstDayOfWeek);
    }

    public static bool IsDateInRangeOf(DateTimeOffset date, DateTimeOffset from, DateTimeOffset? to)
    {
        date = date.Date;
        to = to?.Date ?? DateTime.MaxValue;
        from = from.Date;
        return date <= to && date >= from;
    }

    public static bool IsHourInRangeOf(Tuple<int, int> left, Tuple<int, int> right)
    {
        return left.Item2 > right.Item1 && left.Item1 < right.Item2;
    }
}