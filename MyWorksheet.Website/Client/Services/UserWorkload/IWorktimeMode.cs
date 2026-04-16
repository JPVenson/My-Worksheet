using System;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.UserWorkload;

namespace MyWorksheet.Website.Client.Services.UserWorkload;

public interface IWorktimeMode
{
    string Name { get; }
    int Key { get; }
    GetUserWorkloadViewModel Normalise(GetUserWorkloadViewModel data);
    int GetWeekWorktime(GetUserWorkloadViewModel data);
    int GetDayWorktime(GetUserWorkloadViewModel data, DayOfWeek dayOfWeek);
}