using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.UserWorkload;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.UserWorkload;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.TimeTracking.Shared;

public partial class WorktimeEditComponent
{
    [Inject]
    public UserWorkloadService UserWorkloadService { get; set; }

    private IWorktimeMode _worktimeMode;
    private GetUserWorkloadViewModel _workload;

    [Parameter]
    public IWorktimeMode WorktimeMode
    {
        get { return _worktimeMode; }
        set
        {
            SetProperty(ref _worktimeMode, value, WorktimeModeChanged);
        }
    }

    [Parameter]
    public GetUserWorkloadViewModel Workload
    {
        get { return _workload; }
        set
        {
            SetProperty(ref _workload, value, WorkloadChanged);
            Render();
        }
    }

    [Parameter]
    public EventCallback<IWorktimeMode> WorktimeModeChanged { get; set; }
    [Parameter]
    public EventCallback<GetUserWorkloadViewModel> WorkloadChanged { get; set; }

    public int WeekWorktime
    {
        get { return Workload.WeekWorktime; }
        set
        {
            Workload.WeekWorktime = value;
            SendPropertyChanged();
        }
    }

    public bool IsDayWorkTimeMonday
    {
        get { return Workload?.DayWorkTimeMonday.HasValue == true; }
        set
        {
            Workload.DayWorkTimeMonday = value ? WorktimeMode.GetDayWorktime(Workload, DayOfWeek.Monday) : null;
            SendPropertyChanged();
        }
    }

    public bool IsDayWorkTimeTuesday
    {
        get { return Workload?.DayWorkTimeTuesday.HasValue == true; }
        set
        {
            Workload.DayWorkTimeTuesday = value ? WorktimeMode.GetDayWorktime(Workload, DayOfWeek.Tuesday) : null;
            SendPropertyChanged();
        }
    }
    public bool IsDayWorkTimeWednesday
    {
        get { return Workload?.DayWorkTimeWednesday.HasValue == true; }
        set
        {
            Workload.DayWorkTimeWednesday = value ? WorktimeMode.GetDayWorktime(Workload, DayOfWeek.Wednesday) : null;
            SendPropertyChanged();
        }
    }
    public bool IsDayWorkTimeThursday
    {
        get { return Workload?.DayWorkTimeThursday.HasValue == true; }
        set
        {
            Workload.DayWorkTimeThursday = value ? WorktimeMode.GetDayWorktime(Workload, DayOfWeek.Thursday) : null;
            SendPropertyChanged();
        }
    }
    public bool IsDayWorkTimeFriday
    {
        get { return Workload?.DayWorkTimeFriday.HasValue == true; }
        set
        {
            Workload.DayWorkTimeFriday = value ? WorktimeMode.GetDayWorktime(Workload, DayOfWeek.Friday) : null;
            SendPropertyChanged();
        }
    }
    public bool IsDayWorkTimeSaturday
    {
        get { return Workload?.DayWorkTimeSaturday.HasValue == true; }
        set
        {
            Workload.DayWorkTimeSaturday = value ? WorktimeMode.GetDayWorktime(Workload, DayOfWeek.Saturday) : null;
            SendPropertyChanged();
        }
    }
    public bool IsDayWorkTimeSunday
    {
        get { return Workload?.DayWorkTimeSunday.HasValue == true; }
        set
        {
            Workload.DayWorkTimeSunday = value ? WorktimeMode.GetDayWorktime(Workload, DayOfWeek.Sunday) : null;
            SendPropertyChanged();
        }
    }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        WorktimeMode = WorktimeMode ?? UserWorkloadService.Modes.First(e => e.Key == Workload.WorkTimeMode);
    }
}