using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using MyWorksheet.Website.Client.Components.Form;
using MyWorksheet.Website.Client.Services.ChargeRate;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Repository;
using MyWorksheet.Website.Client.Services.WaiterIndicator;
using MyWorksheet.Website.Client.Services.WorksheetTracker;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.TimeTracking.Worksheet.Tracker;

public partial class WorksheetTrackerPluginComponent
{
    public WorksheetTrackerPluginComponent()
    {

    }

    [Inject]
    public HttpService HttpService { get; set; }

    [Inject]
    public WorksheetTrackerService WorksheetTrackerService { get; set; }

    [Inject]
    public ChargeRateService ChargeRateService { get; set; }

    [Inject]
    public ICacheRepository<WorksheetModel> WorksheetCache { get; set; }

    [Parameter]
    public Guid WorksheetId { get; set; }

    public WorksheetModel WorksheetModel { get; set; }

    private WorksheetTimeTrackerViewModel _tracker;
    private string _commentValue;

    [Parameter]
    public WorksheetTimeTrackerViewModel Tracker
    {
        get { return _tracker; }
        set { SetProperty(ref _tracker, value, TrackerChanged); }
    }

    [Parameter]
    public EventCallback<WorksheetTimeTrackerViewModel> TrackerChanged { get; set; }

    public bool UpdatingCommentValue { get; set; }
    public ValueState<string> CommentValue { get; set; }

    public IEnumerable<ProjectItemRateViewModel> ProjectItemRateViewModels { get; set; }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        WorksheetModel = await WorksheetCache.Cache.Find(WorksheetId);
        var rates = (await ChargeRateService.GetRatesForProject(WorksheetModel.IdProject)).Where(e => !e.Hidden).ToArray();
        ProjectItemRateViewModels = rates;
        System.Console.WriteLine($"Tracker: [{rates.Length}] any hidden [{rates.Any(e => e.Hidden)}]");
        WhenChanged(WorksheetTrackerService)
            .Then(() =>
            {
                Tracker = WorksheetTrackerService.Trackers.FirstOrDefault(e => e.IdWorksheet == WorksheetId);
                CommentValue = Tracker?.Comment ?? string.Empty;
                Render();
            })
            .Trigger();
        WhenChanged(ChargeRateService)
            .ThenRefresh(this);
        AddDisposable(WorksheetTrackerService.OnFullMinute.Register(Render));
    }

    private async Task StartTrack(ProjectItemRateViewModel chargeRate)
    {
        ServerErrorManager.Clear();
        using (WaiterService.WhenDisposed())
        {
            ServerErrorManager.Eval(await WorksheetTrackerService.StartTrack(WorksheetId, chargeRate.ProjectItemRateId));
        }
        ServerErrorManager.DisplayStatus();
    }

    private async Task AbortTracker()
    {
        ServerErrorManager.Clear();
        using (WaiterService.WhenDisposed())
        {
            ServerErrorManager.Eval(await WorksheetTrackerService.CancelTracker(WorksheetId));
        }

        if (ServerErrorManager.ServerErrors.Any())
        {
            ServerErrorManager.DisplayStatus();
        }
        else
        {
            WaiterService.DisplayBadge(new BadgeDisplay()
            {
                Icon = "fas fa-exclamation-circle fa-2x text-warning",
                Text = "Common/Canceled"
            });
        }
    }

    private async Task SaveTracker()
    {
        ServerErrorManager.Clear();
        using (WaiterService.WhenDisposed())
        {
            var mergeReport = ServerErrorManager
                .Eval(await HttpService.WorksheetItemApiAccess.MergeReport(Tracker.StartTime, DateTimeOffset.UtcNow, WorksheetId));
            if (mergeReport.Success)
            {
                if (mergeReport.Object.Overlapping.Any())
                {
                    WaiterService.DisplayBadge(new BadgeDisplay()
                    {
                        Icon = "fas fa-exclamation-circle fa-2x text-warning",
                        Text = "WorksheetItem/Overlapping"
                    });
                    return;
                }
            }

            var apiResult = ServerErrorManager.Eval(await WorksheetTrackerService.EndTracker(WorksheetId));
            if (apiResult.Success)
            {
                Tracker = null;
            }

        }
        ServerErrorManager.DisplayStatus();
    }

    private async Task CommentChanged(ValueChangedEventArgs<string> changedEventArgs)
    {
        if (changedEventArgs.Same || UpdatingCommentValue || !CommentValue.IsObjectDirty)
        {
            return;
        }

        UpdatingCommentValue = true;
        StateHasChanged();
        await HttpService.ProjectTrackerApiAccess.UpdateTrack(WorksheetId, CommentValue.Entity);
        CommentValue.SetPristine();
        UpdatingCommentValue = false;
        StateHasChanged();
    }
}