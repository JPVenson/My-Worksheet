using System;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Invites;

public class AssosiationInviteModel : ViewModelBase
{
    private Guid _assosiationInvitationId;

    private string _externalId;

    private Guid _idRequestingUser;

    private Guid _idUserAssosiatedRoleLookup;

    private bool _revoked;

    private DateTime? _revokedDate;

    private string _revokeReason;

    private bool _validOnce;

    private DateTime? _validUntil;

    public Guid AssosiationInvitationId
    {
        get { return _assosiationInvitationId; }
        set { SetProperty(ref _assosiationInvitationId, value); }
    }

    public string ExternalId
    {
        get { return _externalId; }
        set { SetProperty(ref _externalId, value); }
    }

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

    public bool Revoked
    {
        get { return _revoked; }
        set { SetProperty(ref _revoked, value); }
    }

    public DateTime? RevokedDate
    {
        get { return _revokedDate; }
        set { SetProperty(ref _revokedDate, value); }
    }

    public string RevokeReason
    {
        get { return _revokeReason; }
        set { SetProperty(ref _revokeReason, value); }
    }

    public Guid IdRequestingUser
    {
        get { return _idRequestingUser; }
        set { SetProperty(ref _idRequestingUser, value); }
    }

    public Guid IdUserAssosiatedRoleLookup
    {
        get { return _idUserAssosiatedRoleLookup; }
        set { SetProperty(ref _idUserAssosiatedRoleLookup, value); }
    }

    public override Guid? GetModelIdentifier()
    {
        return AssosiationInvitationId;
    }
}