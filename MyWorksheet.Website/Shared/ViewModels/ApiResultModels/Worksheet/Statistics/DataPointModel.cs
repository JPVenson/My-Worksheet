namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Statistics
{
    public class DataPointModel : ViewModelBase
    {
        private double? _dataValue;

        public double? DataValue
        {
            get { return _dataValue; }
            set { SetProperty(ref _dataValue, value); }
        }
    }
}