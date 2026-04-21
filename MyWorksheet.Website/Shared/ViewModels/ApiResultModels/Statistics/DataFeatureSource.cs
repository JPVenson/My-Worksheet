namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Statistics;

public static class DataFeatureSource
{
    public static DataFeature<bool> ShowLines { get; } = new DataFeature<bool>("showLine");
    public static DataFeature<bool> SpanGaps { get; } = new DataFeature<bool>("spanGaps");


    public static DataSample Feature<TValue>(this DataSample sample, DataFeature<TValue> feature, TValue value)
    {
        var constructedFeature = feature.Construct(value);
        sample.Features[constructedFeature.Key] = constructedFeature.Value;
        return sample;
    }
}