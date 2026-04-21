using System;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Dashboard;

public class InvoiceViewModel : ViewModelBase
{
    private DateTimeOffset _dateOfSubmit;

    private GetProjectModel _project;

    private decimal _timesWorked;

    private WorksheetModel _worksheet;

    public GetProjectModel Project
    {
        get { return _project; }
        set { SetProperty(ref _project, value); }
    }

    public WorksheetModel Worksheet
    {
        get { return _worksheet; }
        set { SetProperty(ref _worksheet, value); }
    }

    public DateTimeOffset DateOfSubmit
    {
        get { return _dateOfSubmit; }
        set { SetProperty(ref _dateOfSubmit, value); }
    }

    public decimal TimesWorked
    {
        get { return _timesWorked; }
        set { SetProperty(ref _timesWorked, value); }
    }
}