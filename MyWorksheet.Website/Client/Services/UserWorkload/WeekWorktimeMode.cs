using System;
using System.Collections.Generic;
using System.Linq;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.UserWorkload;

namespace MyWorksheet.Website.Client.Services.UserWorkload;

public class WeekWorktimeMode : IWorktimeMode
{
    public WeekWorktimeMode()
    {
        Name = "PerWeek";
        Key = 2;
    }

    public string Name { get; }
    public int Key { get; }
    public GetUserWorkloadViewModel Normalise(GetUserWorkloadViewModel data)
    {
        return data;
    }

    public int GetWeekWorktime(GetUserWorkloadViewModel data)
    {
        return data.WeekWorktime;
    }

    public int GetDayWorktime(GetUserWorkloadViewModel data, DayOfWeek dayOfWeek)
    {
        var workTime = new Dictionary<DayOfWeek, double?>();
        workTime.Add(DayOfWeek.Monday, data.DayWorkTimeMonday);
        workTime.Add(DayOfWeek.Tuesday, data.DayWorkTimeTuesday);
        workTime.Add(DayOfWeek.Wednesday, data.DayWorkTimeWednesday);
        workTime.Add(DayOfWeek.Thursday, data.DayWorkTimeThursday);
        workTime.Add(DayOfWeek.Friday, data.DayWorkTimeFriday);
        workTime.Add(DayOfWeek.Saturday, data.DayWorkTimeSaturday);
        workTime.Add(DayOfWeek.Sunday, data.DayWorkTimeSunday);

        var dayWorktime = workTime[dayOfWeek];
        if (dayWorktime.HasValue == false)
        {
            return 0;
        }

        return (data.WeekWorktime / workTime.Count(e => e.Value.HasValue));
    }
}