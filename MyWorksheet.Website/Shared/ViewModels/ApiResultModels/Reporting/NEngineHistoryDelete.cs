using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting;

public class NEngineHistoryDelete : ViewModelBase
{
    private List<Guid> _ids;

    public List<Guid> Ids
    {
        get { return _ids; }
        set { SetProperty(ref _ids, value); }
    }
}