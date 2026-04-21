using System;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Statistics;

public struct DataLine : IStatisticsAggregator<DataLine>
{
    private DataLine(LabelOversight label, LabelOversight sortLabel, DataPoint value)
    {
        Label = label;
        SortLabel = sortLabel;
        DataPoint = value;
    }

    public DataLine(LabelOversight label, LabelOversight sortLabel, decimal value)
        : this(label, sortLabel, (double)value)
    {
    }

    public DataLine(LabelOversight label, LabelOversight sortLabel, double value)
        : this(label, sortLabel, new DataPoint(value))
    {
    }

    public DataLine(LabelOversight label, LabelOversight sortLabel, int value)
        : this(label, sortLabel, (double)value)
    {
    }

    public DataLine(LabelOversight label, decimal value)
        : this(label, label, (double)value)
    {
    }

    public DataLine(LabelOversight label, double value)
        : this(label, label, value)
    {
    }

    public DataLine(LabelOversight label, int value)
        : this(label, label, (double)value)
    {
    }

    public DataLine(LabelOversight label) : this(label, label)
    {
    }

    public DataLine(LabelOversight label, LabelOversight sortLabel)
        : this(label, sortLabel, double.NaN)
    {
    }

    public LabelOversight Label { get; }
    public LabelOversight SortLabel { get; }
    public DataPoint DataPoint { get; }

    public DataLine Aggregate(DataLine with, AggregationStrategy how)
    {
        switch (how)
        {
            case AggregationStrategy.Additive:
            case AggregationStrategy.Average:
                return new DataLine(Label, SortLabel, DataPoint.Aggregate(with.DataPoint, how));
            default:
                throw new ArgumentOutOfRangeException(nameof(how), how, null);
        }
    }
}