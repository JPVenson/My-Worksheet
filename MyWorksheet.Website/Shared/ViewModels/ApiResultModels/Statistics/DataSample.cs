using System;
using System.Collections.Generic;
using System.Linq;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Statistics
{
    /// <summary>
    ///     Defines a single line of values within the Dimension
    /// </summary>
    /// <seealso
    ///     cref="MyWorksheet.Public.Models.ViewModel.ApiResultModels.Statistics.IStatisticsAggregator{MyWorksheet.Public.Models.ViewModel.ApiResultModels.Statistics.DataSample}" />
    public class DataSample : ViewModelBase,
        IStatisticsAggregator<DataSample>
    {
        public DataSample(LabelOversight label, LabelOversight sortLabel)
        {
            DataLines = new List<DataLine>();
            Label = label;
            SortLabel = sortLabel;
            Features = new Dictionary<string, object>();
        }

        public DataSample(LabelOversight label) : this(label, label)
        {
        }

        public LabelOversight Label { get; }
        public LabelOversight SortLabel { get; }
        public List<DataLine> DataLines { get; private set; }
        public IDictionary<string, object> Features { get; }

        public DataSample Aggregate(DataSample with, AggregationStrategy how)
        {
            switch (how)
            {
                case AggregationStrategy.Additive:
                case AggregationStrategy.Average:
                    var aggregateded = DataLines.Concat(with.DataLines)
                        .GroupBy(e => e.Label.Name)
                        .Select(f => f.Aggregate((e, g) => e.Aggregate(g, how)));

                    //var aggregateded = DataLines
                    //	.Join(with.DataLines, f => f.Label.Name, f => f.Label.Name, (left, right) => new {left, right})
                    //	.Select(e => e.left.Aggregate(e.right, how))
                    //	.ToArray();

                    return new DataSample(Label, SortLabel)
                    {
                        DataLines = aggregateded.ToList()
                    };
                case AggregationStrategy.Duplicative:
                    return new DataSample(Label, SortLabel)
                    {
                        DataLines = DataLines.Concat(with.DataLines).ToList()
                    };
                case AggregationStrategy.AddWhereExistsAndDuplicateWhereNot:
                    var aggregated = DataLines.Concat(with.DataLines)
                        .GroupBy(e => e.Label.Name)
                        .Select(f => f.Aggregate((e, g) => e.Aggregate(g, AggregationStrategy.Additive)));

                    //var aggregated = DataLines
                    //	.GroupJoin(with.DataLines, f => f.Label.Name, f => f.Label.Name,
                    //		(left, right) => new {left, right})
                    //	.Select(e => e.right.Aggregate(e.left, (g, f) => g.Aggregate(f, AggregationStrategy.Additive)))
                    //	.ToArray();
                    return new DataSample(Label, SortLabel)
                    {
                        DataLines = aggregated.OrderBy(e => e.SortLabel.Name)
                            .ToList()
                    };
                default:
                    throw new ArgumentOutOfRangeException(nameof(how), how, null);
            }
        }
    }
}