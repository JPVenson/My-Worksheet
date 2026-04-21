namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

public class CreateWorksheetItemResult : ViewModelBase
{
    private object _data;

    private WorksheetItemCreateStatus _status;

    public object Data
    {
        get { return _data; }
        set { SetProperty(ref _data, value); }
    }

    public WorksheetItemCreateStatus Status
    {
        get { return _status; }
        set { SetProperty(ref _status, value); }
    }
}