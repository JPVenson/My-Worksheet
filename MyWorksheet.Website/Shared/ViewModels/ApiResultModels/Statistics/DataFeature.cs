using System.Collections.Generic;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Statistics
{
    public class DataFeature<TValue> : ViewModelBase
    {
        public DataFeature(string featureKey)
        {
            FeatureKey = featureKey;
        }

        public string FeatureKey { get; }

        public KeyValuePair<string, object> Construct(TValue value)
        {
            return new KeyValuePair<string, object>(FeatureKey, value);
        }
    }
}