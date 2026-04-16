using System;
using System.Text;
using MyWorksheet.Website.Server.Models;

namespace MyWorksheet.Helper.Db;

public static class ProjectHelper
{
    public static decimal GetMeanWorktimeForDay(this UserWorkload project, DateTime dayTime)
    {
        var dayOfWeek = dayTime.DayOfWeek;
        return MeanWorktimeForDay(project, dayOfWeek);
    }

    public static decimal MeanWorktimeForDay(this UserWorkload project, DayOfWeek dayOfWeek)
    {
        if (project.WorkTimeMode == 1)
        {
            return WorktimeOfDay(project, dayOfWeek);
        }

        if (project.WorkTimeMode == 2)
        {
            if (WorktimeOfDay(project, dayOfWeek) == -1)
            {
                return -1;
            }

            var workDays = 0;
            if (project.DayWorkTimeMonday != -1)
            {
                workDays++;
            }

            if (project.DayWorkTimeTuesday != -1)
            {
                workDays++;
            }

            if (project.DayWorkTimeWednesday != -1)
            {
                workDays++;
            }

            if (project.DayWorkTimeThursday != -1)
            {
                workDays++;
            }

            if (project.DayWorkTimeFriday != -1)
            {
                workDays++;
            }

            if (project.DayWorkTimeSaturday != -1)
            {
                workDays++;
            }

            if (project.DayWorkTimeSunday != -1)
            {
                workDays++;
            }

            var worktimePerDay = project.WeekWorktime / workDays;
            return worktimePerDay;
        }

        return 0;
    }

    private static decimal WorktimeOfDay(UserWorkload project, DayOfWeek dayTime)
    {
        switch (dayTime)
        {
            case (DayOfWeek)1:
                return project.DayWorkTimeMonday.GetValueOrDefault();
            case (DayOfWeek)2:
                return project.DayWorkTimeTuesday.GetValueOrDefault();
            case (DayOfWeek)3:
                return project.DayWorkTimeWednesday.GetValueOrDefault();
            case (DayOfWeek)4:
                return project.DayWorkTimeThursday.GetValueOrDefault();
            case (DayOfWeek)5:
                return project.DayWorkTimeFriday.GetValueOrDefault();
            case (DayOfWeek)6:
                return project.DayWorkTimeSaturday.GetValueOrDefault();
            case 0:
                return project.DayWorkTimeSunday.GetValueOrDefault();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

public static class TimespanHelper
{
    public static string Humanise(this TimeSpan timespan, bool shortWritten)
    {
        var sb = new StringBuilder();
        if (timespan.Days > 0)
        {
            sb.Append(timespan.Days + (shortWritten ? "d " : "days "));
        }
        if (timespan.TotalHours > 0)
        {
            sb.Append(timespan.Hours + (shortWritten ? "h " : "Hours "));
        }
        if (timespan.TotalMinutes > 0)
        {
            sb.Append(timespan.Minutes + (shortWritten ? "m" : "Minutes "));
        }
        return sb.ToString();
    }
}