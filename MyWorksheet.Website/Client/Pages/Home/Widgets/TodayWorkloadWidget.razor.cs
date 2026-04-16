using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.UserWorkload;
using MyWorksheet.Website.Client.Services.WorksheetTracker;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.UserWorkload;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Home.Widgets;

public partial class TodayWorkloadWidget
{
    [Inject]
    public HttpService HttpService { get; set; }

    [Inject]
    public UserWorkloadService UserWorkloadService { get; set; }

    [Inject]
    public WorksheetTrackerService WorksheetTrackerService { get; set; }

    public GetUserWorkloadViewModel Workload { get; set; }
    public double TrackedMinutesToday { get; set; }
    public double ExpectedMinutesToday { get; set; }
    public IEnumerable<WorksheetTimeTrackerViewModel> RunningTrackers { get; set; }
        = Enumerable.Empty<WorksheetTimeTrackerViewModel>();

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        await WorksheetTrackerService.Trackers.Load();

        Workload = ServerErrorManager.EvalAndUnbox(
            await HttpService.UserWorkloadApiAccess.GetDefaultWorkload());

        if (Workload != null)
        {
            ExpectedMinutesToday = GetExpectedMinutesForToday(Workload);
        }

        RunningTrackers = WorksheetTrackerService.Trackers.ToList();
        TrackedMinutesToday = RunningTrackers
            .Sum(t => (DateTimeOffset.UtcNow - t.StartTime).TotalMinutes);

        WhenChanged(WorksheetTrackerService).Then(() =>
        {
            RunningTrackers = WorksheetTrackerService.Trackers.ToList();
            TrackedMinutesToday = RunningTrackers
                .Sum(t => (DateTimeOffset.UtcNow - t.StartTime).TotalMinutes);
        });

        AddDisposable(WorksheetTrackerService.OnFullMinute.Register(Render));
    }

    internal static double GetExpectedMinutesForToday(GetUserWorkloadViewModel workload)
    {
        int? minutes = DateTime.Today.DayOfWeek switch
        {
            DayOfWeek.Monday => workload.DayWorkTimeMonday,
            DayOfWeek.Tuesday => workload.DayWorkTimeTuesday,
            DayOfWeek.Wednesday => workload.DayWorkTimeWednesday,
            DayOfWeek.Thursday => workload.DayWorkTimeThursday,
            DayOfWeek.Friday => workload.DayWorkTimeFriday,
            DayOfWeek.Saturday => workload.DayWorkTimeSaturday,
            DayOfWeek.Sunday => workload.DayWorkTimeSunday,
            _ => null
        };

        if (minutes.HasValue)
        {
            return minutes.Value;
        }

        // Fall back to daily average from weekly total
        return workload.WeekWorktime / 5.0;
    }
}
