using System;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Administration.ExecuteLater;

public class SchedulerInfo : ViewModelBase
{
    private DateTime _lastRun;
    private bool? _lastRunSuccess;
    private DateTime _nextRun;
    private TaskInfo _taskInfo;
    public string Schedule { get; set; }

    public DateTime NextRun
    {
        get { return _nextRun; }
        set { SetProperty(ref _nextRun, value); }
    }

    public TaskInfo TaskInfo
    {
        get { return _taskInfo; }
        set { SetProperty(ref _taskInfo, value); }
    }

    public DateTime LastRun
    {
        get { return _lastRun; }
        set { SetProperty(ref _lastRun, value); }
    }

    public bool? LastRunSuccess
    {
        get { return _lastRunSuccess; }
        set { SetProperty(ref _lastRunSuccess, value); }
    }
}