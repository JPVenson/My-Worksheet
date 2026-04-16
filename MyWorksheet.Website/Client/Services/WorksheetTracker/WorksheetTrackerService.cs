using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Auth;
using MyWorksheet.Website.Client.Services.ChangeTracking;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Signal;
using MyWorksheet.Website.Client.Util;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Server.Services;
using MyWorksheet.Website.Shared.Services.Activation;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Client.Services.WorksheetTracker;

[SingletonService()]
public class WorksheetTrackerService : RequireInit, ILazyLoadedService
{
    private readonly HttpService _httpService;
    private readonly ChangeTrackingService _changeTrackingService;
    private readonly CurrentUserStore _currentUserStore;

    public WorksheetTrackerService(HttpService httpService,
        ChangeTrackingService changeTrackingService,
        CurrentUserStore currentUserStore)
    {
        _httpService = httpService;
        _changeTrackingService = changeTrackingService;
        _currentUserStore = currentUserStore;
        OnFullMinute = new PubSubEvent();
        var timerTask = Task.Run(async () =>
        {
            while (true)
            {
                var delay = (60 - DateTime.Now.Second) * 1000;
                await Task.Delay(delay);
                await OnFullMinute.Raise();
            }
        });
    }

    public PubSubEvent OnFullMinute { get; set; }

    public IFutureList<WorksheetTimeTrackerViewModel> Trackers { get; set; }

    public Task<ApiResult<WorksheetTimeTrackerViewModel>> CancelTracker(Guid idWorksheet)
    {
        return CancelTracker(Trackers.First(e => e.IdWorksheet == idWorksheet));
    }
    public async Task<ApiResult<WorksheetTimeTrackerViewModel>> CancelTracker(WorksheetTimeTrackerViewModel tracker)
    {
        var abortTrack = await _httpService.ProjectTrackerApiAccess.AbortTrack(tracker.IdWorksheet);
        if (abortTrack.Success)
        {
            Trackers.Remove(tracker);
            OnDataLoaded();
        }

        return abortTrack;
    }

    public async Task<ApiResult<WorksheetTimeTrackerViewModel>> EndTracker(Guid worksheetId,
        DateTimeOffset? fromTime = null,
        DateTimeOffset? toTime = null,
        string comment = null)
    {
        var abortTrack = await _httpService.ProjectTrackerApiAccess.EndTrack(worksheetId,
            fromTime,
            toTime,
            comment);
        if (abortTrack.Success)
        {
            var tracker = Trackers.FirstOrDefault(e => e.IdWorksheet == worksheetId);
            if (tracker != null)
            {
                Trackers.Remove(tracker);
            }
            OnDataLoaded();
        }

        return abortTrack;
    }

    public Task<ApiResult<WorksheetTimeTrackerViewModel>> EndTracker(WorksheetTimeTrackerViewModel tracker)
    {
        return EndTracker(tracker.IdWorksheet, null, null, null);
    }

    public async Task<ApiResult<WorksheetTimeTrackerViewModel>> StartTrack(Guid worksheetId, Guid chargeRateProjectItemRateId)
    {
        var beginTrack = await _httpService.ProjectTrackerApiAccess.BeginTrack(worksheetId, chargeRateProjectItemRateId);
        if (beginTrack.Success)
        {
            Trackers.Add(beginTrack.Object);
            OnDataLoaded();
        }

        return beginTrack;
    }

    public async Task<ApiResult> UpdateTrack(Guid idWorksheet, string comment)
    {
        var beginTrack = await _httpService.ProjectTrackerApiAccess.UpdateTrack(idWorksheet, comment);
        if (beginTrack.Success)
        {
            var tracker = Trackers.First(e => e.IdWorksheet == idWorksheet);
            tracker.Comment = comment;
            OnDataLoaded();
        }

        return beginTrack;
    }

    public override async ValueTask InitAsync()
    {
        await base.InitAsync();
        _changeTrackingService.RegisterTracking(typeof(WorksheetTimeTrackerViewModel), TrackerChanged);
        Trackers = new FutureList<WorksheetTimeTrackerViewModel>(async () => await _httpService.ProjectTrackerApiAccess.GetTracks());
        Trackers.WhenLoaded(OnDataLoaded);
        await _currentUserStore.WhenChanged()
            .UserIsNotAuthenticated(() => Trackers.Clear())
            .Invoke();
    }

    private void TrackerChanged(EntityChangedEventArguments eventArguments)
    {
        switch (eventArguments.ChangeEventTypes)
        {
            case ChangeEventTypes.Added:
                Trackers.Reset();
                OnDataLoaded();
                break;
            case ChangeEventTypes.Removed:
                Trackers.RemoveWhere(e => eventArguments.Ids.Contains(e.WorksheetTrackId));
                OnDataLoaded();
                break;
            case ChangeEventTypes.Changed:
                Trackers.Reset();
                OnDataLoaded();
                break;
        }
    }

    public event EventHandler DataLoaded;

    protected virtual void OnDataLoaded()
    {
        DataLoaded?.Invoke(this, EventArgs.Empty);
    }
}