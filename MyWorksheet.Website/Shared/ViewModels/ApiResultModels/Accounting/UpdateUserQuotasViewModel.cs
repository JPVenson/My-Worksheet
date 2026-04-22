using System;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;

public class UpdateUserQuotasViewModel : ViewModelBase
{
    private UserQuotaViewModel[] _quotas;

    private Guid _userId;

    public Guid UserId
    {
        get { return _userId; }
        set { SetProperty(ref _userId, value); }
    }

    public UserQuotaViewModel[] Quotas
    {
        get { return _quotas; }
        set { SetProperty(ref _quotas, value); }
    }
}