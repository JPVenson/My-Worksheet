using System;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Administration.ExecuteLater;

public class PriorityQueueItemViewModel : ViewModelBase
{
    private string _actionKey;

    private string _dataArguments;

    private DateTimeOffset _dateOfCreation;

    private DateTimeOffset? _dateOfDone;

    private bool _done;

    private string _error;

    private Guid _idCreator;

    private Guid? _idParent;

    private string _level;

    private Guid _priorityQueueItemId;

    private bool _success;

    private string _version;

    public Guid PriorityQueueItemId
    {
        get { return _priorityQueueItemId; }
        set { SetProperty(ref _priorityQueueItemId, value); }
    }

    public Guid? IdParent
    {
        get { return _idParent; }
        set { SetProperty(ref _idParent, value); }
    }

    public Guid IdCreator
    {
        get { return _idCreator; }
        set { SetProperty(ref _idCreator, value); }
    }

    public string ActionKey
    {
        get { return _actionKey; }
        set { SetProperty(ref _actionKey, value); }
    }

    public string DataArguments
    {
        get { return _dataArguments; }
        set { SetProperty(ref _dataArguments, value); }
    }

    public string Version
    {
        get { return _version; }
        set { SetProperty(ref _version, value); }
    }

    public string Level
    {
        get { return _level; }
        set { SetProperty(ref _level, value); }
    }

    public DateTimeOffset DateOfCreation
    {
        get { return _dateOfCreation; }
        set { SetProperty(ref _dateOfCreation, value); }
    }

    public DateTimeOffset? DateOfDone
    {
        get { return _dateOfDone; }
        set { SetProperty(ref _dateOfDone, value); }
    }

    public bool Done
    {
        get { return _done; }
        set { SetProperty(ref _done, value); }
    }

    public bool Success
    {
        get { return _success; }
        set { SetProperty(ref _success, value); }
    }

    public string Error
    {
        get { return _error; }
        set { SetProperty(ref _error, value); }
    }

    public override Guid? GetModelIdentifier()
    {
        return PriorityQueueItemId;
    }
}