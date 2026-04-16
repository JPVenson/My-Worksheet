namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Statistics
{
    public interface IStatisticsAggregator<T>
    {
        T Aggregate(T with, AggregationStrategy how);
    }
}