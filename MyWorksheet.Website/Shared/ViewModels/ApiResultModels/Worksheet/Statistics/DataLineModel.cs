namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Statistics;

public class DataLineModel : ViewModelBase
{
    private DataPointModel _dataPoint;

    private LabelOversightModel _label;

    public LabelOversightModel Label
    {
        get { return _label; }
        set { SetProperty(ref _label, value); }
    }

    public DataPointModel DataPoint
    {
        get { return _dataPoint; }
        set { SetProperty(ref _dataPoint, value); }
    }
}