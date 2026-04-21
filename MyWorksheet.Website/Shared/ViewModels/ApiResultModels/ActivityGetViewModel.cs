using System;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels;

[ObjectTracking("UserActivity")]
public class UserActivityViewModel : ViewModelBase
{
    private string _activityType;
    private string _bodyHtml;
    private DateTime _dateCreated;
    private string _footerHtml;
    private string _headerHtml;
    private bool _hidden;
    private Guid? _idCreator;
    private long? _timeToLive;
    private Guid _userActivityId;

    public Guid UserActivityId
    {
        get { return _userActivityId; }
        set { SetProperty(ref _userActivityId, value); }
    }

    public long? TimeToLive
    {
        get { return _timeToLive; }
        set { SetProperty(ref _timeToLive, value); }
    }

    public DateTime DateCreated
    {
        get { return _dateCreated; }
        set { SetProperty(ref _dateCreated, value); }
    }

    public Guid? IdCreator
    {
        get { return _idCreator; }
        set { SetProperty(ref _idCreator, value); }
    }

    public string HeaderHtml
    {
        get { return _headerHtml; }
        set { SetProperty(ref _headerHtml, value); }
    }

    public string BodyHtml
    {
        get { return _bodyHtml; }
        set { SetProperty(ref _bodyHtml, value); }
    }

    public string FooterHtml
    {
        get { return _footerHtml; }
        set { SetProperty(ref _footerHtml, value); }
    }

    public string ActivityType
    {
        get { return _activityType; }
        set { SetProperty(ref _activityType, value); }
    }

    public bool Hidden
    {
        get { return _hidden; }
        set { SetProperty(ref _hidden, value); }
    }

    public override Guid? GetModelIdentifier()
    {
        return UserActivityId;
    }
}