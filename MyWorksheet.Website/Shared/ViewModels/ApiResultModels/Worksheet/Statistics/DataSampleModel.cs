using System.Collections.Generic;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Statistics
{
    public class DataSampleModel : ViewModelBase
    {
        private List<DataLineModel> _dataLines;

        private LabelOversightModel _label;

        public LabelOversightModel Label
        {
            get { return _label; }
            set { SetProperty(ref _label, value); }
        }

        public List<DataLineModel> DataLines
        {
            get { return _dataLines; }
            set { SetProperty(ref _dataLines, value); }
        }
    }
}