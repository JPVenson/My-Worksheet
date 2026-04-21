using System;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Invites;

public class InviteCreationViewModel : ViewModelBase
{
    private Guid _linkType;

    private bool _validOnce;

    private DateTime? _validUntil;

    public DateTime? ValidUntil
    {
        get { return _validUntil; }
        set { SetProperty(ref _validUntil, value); }
    }

    public bool ValidOnce
    {
        get { return _validOnce; }
        set { SetProperty(ref _validOnce, value); }
    }

    public Guid LinkType
    {
        get { return _linkType; }
        set { SetProperty(ref _linkType, value); }
    }
}