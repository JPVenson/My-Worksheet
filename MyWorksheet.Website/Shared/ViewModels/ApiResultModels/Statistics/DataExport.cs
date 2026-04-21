using System;
using System.Collections.Generic;
using System.Linq;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Statistics;

public class DataExport : ViewModelBase,
    IStatisticsAggregator<DataExport>
{
    private List<DataDimension> _dataDimensions;

    private bool _dataMaybeTruncated;

    public DataExport()
    {
        DataDimensions = new List<DataDimension>();
    }

    public bool DataMaybeTruncated
    {
        get { return _dataMaybeTruncated; }
        set { SetProperty(ref _dataMaybeTruncated, value); }
    }

    public List<DataDimension> DataDimensions
    {
        get { return _dataDimensions; }
        set { SetProperty(ref _dataDimensions, value); }
    }

    public DataExport Aggregate(DataExport with, AggregationStrategy how)
    {
        if (DataDimensions.Any() && !with.DataDimensions.Any())
        {
            return this;
        }
        if (!DataDimensions.Any() && with.DataDimensions.Any())
        {
            return with;
        }
        switch (how)
        {
            case AggregationStrategy.Additive:
            case AggregationStrategy.Average:
            case AggregationStrategy.Duplicative:
            case AggregationStrategy.AddWhereExistsAndDuplicateWhereNot:
                var aggregated = DataDimensions.Concat(with.DataDimensions)
                    .GroupBy(e => e.Label.Name)
                    .Select(f => f.Aggregate((e, g) => e.Aggregate(g, how)));

                //var aggregated = DataDimensions
                //	.Zip(with.DataDimensions, (left, right) => (left, right))
                //	.Select(e => e.left?.Aggregate(e.right, how) ?? e.right ?? e.left)
                //	.ToArray();
                return new DataExport
                {
                    DataMaybeTruncated = DataMaybeTruncated || with.DataMaybeTruncated,
                    DataDimensions = aggregated.ToList()
                };
            default:
                throw new ArgumentOutOfRangeException(nameof(how), how, null);
        }
    }
}