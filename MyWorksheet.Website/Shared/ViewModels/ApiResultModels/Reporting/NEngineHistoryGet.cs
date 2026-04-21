using System;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Reports;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting;

public class NEngineHistoryGet : ViewModelBase
{
    private ScheduleReportModel _argumentsRepresentation;

    private DateTime _dateCreated;
    private DateTime? _dateRun;
    private string _errorText;
    private Guid _idNEngineTemplate;
    private Guid? _idProcessor;
    private Guid? _idStoreageEntry;
    private bool _isDone;
    private bool _isFaulted;
    private bool _isObsolete;
    private bool _isPreview;
    private Guid _nEngineRunningTaskId;

    public Guid NEngineRunningTaskId
    {
        get { return _nEngineRunningTaskId; }
        set { SetProperty(ref _nEngineRunningTaskId, value); }
    }

    public ScheduleReportModel ArgumentsRepresentation
    {
        get { return _argumentsRepresentation; }
        set { SetProperty(ref _argumentsRepresentation, value); }
    }

    public bool IsDone
    {
        get { return _isDone; }
        set { SetProperty(ref _isDone, value); }
    }

    public bool IsPreview
    {
        get { return _isPreview; }
        set { SetProperty(ref _isPreview, value); }
    }

    public bool IsFaulted
    {
        get { return _isFaulted; }
        set { SetProperty(ref _isFaulted, value); }
    }

    public bool IsObsolete
    {
        get { return _isObsolete; }
        set { SetProperty(ref _isObsolete, value); }
    }

    public string ErrorText
    {
        get { return _errorText; }
        set { SetProperty(ref _errorText, value); }
    }

    public DateTime DateCreated
    {
        get { return _dateCreated; }
        set { SetProperty(ref _dateCreated, value); }
    }

    public DateTime? DateRun
    {
        get { return _dateRun; }
        set { SetProperty(ref _dateRun, value); }
    }

    public Guid IdNEngineTemplate
    {
        get { return _idNEngineTemplate; }
        set { SetProperty(ref _idNEngineTemplate, value); }
    }

    public Guid? IdStoreageEntry
    {
        get { return _idStoreageEntry; }
        set { SetProperty(ref _idStoreageEntry, value); }
    }

    public Guid? IdProcessor
    {
        get { return _idProcessor; }
        set { SetProperty(ref _idProcessor, value); }
    }

    public override Guid? GetModelIdentifier()
    {
        return NEngineRunningTaskId;
    }
}