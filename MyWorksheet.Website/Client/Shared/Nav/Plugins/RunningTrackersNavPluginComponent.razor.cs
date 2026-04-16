using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Components.Form;
using MyWorksheet.Website.Client.Services.Http;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Repository;
using MyWorksheet.Website.Client.Services.WorksheetTracker;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Shared.Nav.Plugins;

public partial class RunningTrackersNavPluginComponent
{
    public RunningTrackersNavPluginComponent()
    {
        Tracker = new List<TrackerViewModel>();
    }

    [Inject]
    public WorksheetTrackerService WorksheetTrackerService { get; set; }

    [Inject]
    public HttpService HttpService { get; set; }

    [Inject]
    public ICacheRepository<GetProjectModel> ProjectsRepository { get; set; }

    [Inject]
    public ICacheRepository<WorksheetModel> WorksheetRepository { get; set; }

    public IList<TrackerViewModel> Tracker { get; set; }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        WhenChanged(WorksheetTrackerService)
            .Then(RefreshTracker)
            .Trigger();
        await WorksheetTrackerService.Trackers.Load();
        AddDisposable(WorksheetTrackerService.OnFullMinute.Register(Render));
    }

    private async void RefreshTracker()
    {
        Tracker.Clear();
        if (!WorksheetTrackerService.Trackers.Any())
        {
            Render();
            return;
        }

        var projectIds = WorksheetTrackerService.Trackers.Select(e => e.IdProject).ToArray();
        var worksheetIds = WorksheetTrackerService.Trackers.Select(e => e.IdWorksheet).ToArray();

        await ProjectsRepository.Cache.FindBy(e => projectIds.Contains(e.ProjectId), e => e.GetList(projectIds));
        await WorksheetRepository.Cache.FindBy(e => worksheetIds.Contains(e.WorksheetId), e => e.GetList(worksheetIds));

        foreach (var worksheetTimeTrackerViewModel in WorksheetTrackerService.Trackers)
        {
            var tracker = new TrackerViewModel();
            tracker.Tracker = worksheetTimeTrackerViewModel;
            tracker.Worksheet = await WorksheetRepository.Cache.Find(worksheetTimeTrackerViewModel.IdWorksheet);
            tracker.Project = await ProjectsRepository.Cache.Find(worksheetTimeTrackerViewModel.IdProject);
            tracker.TrackerComment = new ValueState<string>(worksheetTimeTrackerViewModel.Comment ?? "");
            Tracker.Add(tracker);
        }
        Render();
    }

    private async Task UpdateTrackerComment(TrackerViewModel tracker)
    {
        ServerErrorManager.Clear();
        if (!tracker.TrackerComment.IsObjectDirty)
        {
            return;
        }

        try
        {
            tracker.IsUpdating = true;
            var updateTrack = ServerErrorManager.Eval(await WorksheetTrackerService.UpdateTrack(tracker.Tracker.IdWorksheet,
                tracker.TrackerComment.Entity));
            if (updateTrack.Success)
            {
                tracker.Tracker.Comment = tracker.TrackerComment.Entity;
                tracker.TrackerComment.SetPristine();
                WaiterService.DisplayOk();
            }
        }
        finally
        {
            await Task.Delay(1000);
            tracker.IsUpdating = false;
        }
    }

    private async Task CancelTracker(TrackerViewModel tracker)
    {
        var result = ServerErrorManager.Eval(await WorksheetTrackerService.CancelTracker(tracker.Tracker));
        if (result.Success)
        {
            Tracker.Remove(tracker);
        }
    }

    private async Task SaveTracker(TrackerViewModel tracker)
    {
        ServerErrorManager.Clear();
        if (tracker.TrackerComment.IsObjectDirty)
        {
            await UpdateTrackerComment(tracker);
        }

        var apiResult = ServerErrorManager.Eval(await WorksheetTrackerService.EndTracker(tracker.Tracker));
        if (apiResult.Success)
        {
            Tracker.Remove(tracker);
        }
        ServerErrorManager.DisplayStatus();
    }
}

public class TrackerViewModel
{
    public GetProjectModel Project { get; set; }
    public WorksheetModel Worksheet { get; set; }
    public WorksheetTimeTrackerViewModel Tracker { get; set; }

    public ValueState<string> TrackerComment { get; set; }

    public string Comment
    {
        get { return TrackerComment.Entity; }
        set { TrackerComment.Entity = value; }
    }

    public bool IsUpdating { get; set; }

}