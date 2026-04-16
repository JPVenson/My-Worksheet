using System;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Statistics
{
    public struct DataPoint : IStatisticsAggregator<DataPoint>
    {
        public DataPoint(double dataValue)
        {
            DataValue = dataValue;
        }

        public DataPoint(double? dataValue)
        {
            DataValue = dataValue;
        }

        public double? DataValue { get; }

        public DataPoint Aggregate(DataPoint with, AggregationStrategy how)
        {
            switch (how)
            {
                case AggregationStrategy.Additive:
                    return new DataPoint(with.DataValue + DataValue);
                case AggregationStrategy.Average:
                    return new DataPoint((with.DataValue + DataValue) / 2);
                default:
                    throw new ArgumentOutOfRangeException(nameof(how), how, null);
            }
        }
    }
}