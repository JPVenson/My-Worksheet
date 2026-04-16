using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Services.WorksheetTracker;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Home.Widgets;

public partial class RunningTrackersWidget
{
    [Inject]
    public WorksheetTrackerService WorksheetTrackerService { get; set; }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        await WorksheetTrackerService.Trackers.Load();
        WhenChanged(WorksheetTrackerService).ThenRefresh(this);
    }
}
