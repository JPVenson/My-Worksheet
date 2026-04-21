using System;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Payment;

public class PostFeature : GetFeature
{
    private string _comment;

    private Guid _idPromisedFeatureRegion;

    private int? _limitTo;

    private int? _limitToUser;

    private DateTime? _validFrom;

    private DateTime? _validTo;

    public Guid IdPromisedFeatureRegion
    {
        get { return _idPromisedFeatureRegion; }
        set { SetProperty(ref _idPromisedFeatureRegion, value); }
    }

    public string Comment
    {
        get { return _comment; }
        set { SetProperty(ref _comment, value); }
    }

    public DateTime? ValidFrom
    {
        get { return _validFrom; }
        set { SetProperty(ref _validFrom, value); }
    }

    public DateTime? ValidTo
    {
        get { return _validTo; }
        set { SetProperty(ref _validTo, value); }
    }

    public int? LimitTo
    {
        get { return _limitTo; }
        set { SetProperty(ref _limitTo, value); }
    }

    public int? LimitToUser
    {
        get { return _limitToUser; }
        set { SetProperty(ref _limitToUser, value); }
    }
}