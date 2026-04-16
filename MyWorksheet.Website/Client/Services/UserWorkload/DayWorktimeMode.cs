using System;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.UserWorkload;

namespace MyWorksheet.Website.Client.Services.UserWorkload;

public class DayWorktimeMode : IWorktimeMode
{
    public DayWorktimeMode()
    {
        Key = 1;
        Name = "PerDay";
    }

    public string Name { get; }
    public int Key { get; }
    public GetUserWorkloadViewModel Normalise(GetUserWorkloadViewModel data)
    {
        data.WeekWorktime = -1;
        return data;
    }

    public int GetWeekWorktime(GetUserWorkloadViewModel data)
    {
        return (data.DayWorkTimeMonday.GetValueOrDefault() +
                data.DayWorkTimeTuesday.GetValueOrDefault() +
                data.DayWorkTimeWednesday.GetValueOrDefault() +
                data.DayWorkTimeThursday.GetValueOrDefault() +
                data.DayWorkTimeFriday.GetValueOrDefault() +
                data.DayWorkTimeSaturday.GetValueOrDefault() +
                data.DayWorkTimeSunday.GetValueOrDefault());
    }

    public int GetDayWorktime(GetUserWorkloadViewModel data, DayOfWeek dayOfWeek)
    {
        switch (dayOfWeek)
        {
            case DayOfWeek.Sunday:
                return data.DayWorkTimeSunday.GetValueOrDefault();
            case DayOfWeek.Monday:
                return data.DayWorkTimeMonday.GetValueOrDefault();
            case DayOfWeek.Tuesday:
                return data.DayWorkTimeTuesday.GetValueOrDefault();
            case DayOfWeek.Wednesday:
                return data.DayWorkTimeWednesday.GetValueOrDefault();
            case DayOfWeek.Thursday:
                return data.DayWorkTimeThursday.GetValueOrDefault();
            case DayOfWeek.Friday:
                return data.DayWorkTimeFriday.GetValueOrDefault();
            case DayOfWeek.Saturday:
                return data.DayWorkTimeSaturday.GetValueOrDefault();
            default:
                throw new ArgumentOutOfRangeException(nameof(dayOfWeek), dayOfWeek, null);
        }
    }
}