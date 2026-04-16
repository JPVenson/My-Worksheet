using System.Collections.Generic;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Statistics
{
    public class DataExportModel : ViewModelBase
    {
        private List<DataDimensionModel> _dataDimensions;

        private bool _dataMaybeTruncated;

        public bool DataMaybeTruncated
        {
            get { return _dataMaybeTruncated; }
            set { SetProperty(ref _dataMaybeTruncated, value); }
        }

        //public List<LabelOversight> Labels { get; set; }
        public List<DataDimensionModel> DataDimensions
        {
            get { return _dataDimensions; }
            set { SetProperty(ref _dataDimensions, value); }
        }
    }

    //public enum DisplayTypes
    //{
    //	Line,
    //	Bar,
    //	Pie,
    //	Radar,
    //	PolarArea,
    //	Doughnut,
    //	HorizontalBar
    //}
}