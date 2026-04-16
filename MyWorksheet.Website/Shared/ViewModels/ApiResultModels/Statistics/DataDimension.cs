using System;
using System.Collections.Generic;
using System.Linq;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Statistics;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Statistics
{
    /// <summary>
    ///     Dimensions are the seperation of a group of Samples that are displayed together
    /// </summary>
    /// <seealso
    ///     cref="MyWorksheet.Public.Models.ViewModel.ApiResultModels.Statistics.IStatisticsAggregator{MyWorksheet.Public.Models.ViewModel.ApiResultModels.Statistics.DataDimension}" />
    public class DataDimension : ViewModelBase,
        IStatisticsAggregator<DataDimension>
    {
        private bool _dataMaybeTruncated;
        private List<DataSample> _dataSamples;
        private DisplayTypes _displayAs = DisplayTypes.Line;
        private LabelOversight _label;
        private LabelOversight _sortLabel;

        public DataDimension(LabelOversight label, LabelOversight sortLabel)
        {
            Label = label;
            SortLabel = sortLabel;
            DataSamples = new List<DataSample>();
        }

        public DataDimension(LabelOversight label) : this(label, label)
        {
        }

        public bool DataMaybeTruncated
        {
            get { return _dataMaybeTruncated; }
            set { SetProperty(ref _dataMaybeTruncated, value); }
        }

        public LabelOversight Label
        {
            get { return _label; }
            set { SetProperty(ref _label, value); }
        }

        public LabelOversight SortLabel
        {
            get { return _sortLabel; }
            set { SetProperty(ref _sortLabel, value); }
        }

        public List<DataSample> DataSamples
        {
            get { return _dataSamples; }
            set { SetProperty(ref _dataSamples, value); }
        }

        public DisplayTypes DisplayAs
        {
            get { return _displayAs; }
            set { SetProperty(ref _displayAs, value); }
        }

        public DataDimension Aggregate(DataDimension with, AggregationStrategy how)
        {
            if (with == null)
            {
                return this;
            }

            switch (how)
            {
                case AggregationStrategy.Additive:
                case AggregationStrategy.Average:
                case AggregationStrategy.AddWhereExistsAndDuplicateWhereNot:
                    var aggregated = DataSamples.Concat(with.DataSamples)
                        .GroupBy(e => e.Label.Name)
                        .Select(f => f.Aggregate((e, g) => e.Aggregate(g, how)));

                    //var aggregated = DataSamples
                    //	.GroupBy(with.DataSamples, f => f.Label.Name, f => f.Label.Name,
                    //		(left, right) => new {left, right})
                    //	.Select(e => e.right.Aggregate(e.left, (g, f) => g.Aggregate(f, how)))
                    //	.ToArray();
                    //;

                    return new DataDimension(Label, SortLabel)
                    {
                        DataMaybeTruncated = DataMaybeTruncated || with.DataMaybeTruncated,
                        DataSamples = aggregated.ToList(),
                        DisplayAs = DisplayAs
                    };
                case AggregationStrategy.Duplicative:
                    return new DataDimension(Label, SortLabel)
                    {
                        DataMaybeTruncated = DataMaybeTruncated || with.DataMaybeTruncated,
                        DataSamples = DataSamples.Concat(with.DataSamples).ToList(),
                        DisplayAs = DisplayAs
                    };
                default:
                    throw new ArgumentOutOfRangeException(nameof(how), how, null);
            }
        }
    }
}