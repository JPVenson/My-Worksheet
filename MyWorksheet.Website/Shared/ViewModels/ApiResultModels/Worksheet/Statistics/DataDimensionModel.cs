using System.Collections.Generic;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Statistics
{
    public class DataDimensionModel : ViewModelBase
    {
        private bool _dataMaybeTruncated;

        private List<DataSampleModel> _dataSamples;

        private string _displayAs;

        private LabelOversightModel _label;

        public bool DataMaybeTruncated
        {
            get { return _dataMaybeTruncated; }
            set { SetProperty(ref _dataMaybeTruncated, value); }
        }

        public LabelOversightModel Label
        {
            get { return _label; }
            set { SetProperty(ref _label, value); }
        }

        public List<DataSampleModel> DataSamples
        {
            get { return _dataSamples; }
            set { SetProperty(ref _dataSamples, value); }
        }

        public string DisplayAs
        {
            get { return _displayAs; }
            set { SetProperty(ref _displayAs, value); }
        }
    }
}