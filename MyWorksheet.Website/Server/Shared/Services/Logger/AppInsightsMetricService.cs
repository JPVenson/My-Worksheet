using System.Collections.Generic;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using MyWorksheet.Website.Server.Shared.Services.Logging.Default;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Shared.Services.Logger;

[SingletonService(typeof(IAppMetricsLogger))]
public class AppInsightsMetricService : IAppMetricsLogger
{
    public AppInsightsMetricService(IAppInsightsProviderService appInsightsProviderService)
    {
        MetricBuffer = new CircularBuffer<KeyValuePair<string, int>>(300);
        AppInsights = appInsightsProviderService;
        if (AppInsights.TelemetryClient == null)
        {
            return;
        }
        foreach (var keyValuePair in MetricBuffer)
        {
            AppInsights.TelemetryClient.TrackMetric(keyValuePair.Key, keyValuePair.Value);
        }
        MetricBuffer.Clear();
    }

    public IAppInsightsProviderService AppInsights { get; set; }
    public CircularBuffer<KeyValuePair<string, int>> MetricBuffer { get; private set; }

    public void TrackMetric(string key, int metric)
    {
        if (AppInsights?.TelemetryClient == null)
        {
            MetricBuffer.Add(new KeyValuePair<string, int>(key, metric));
        }
        else
        {
            AppInsights.TelemetryClient.TrackMetric(key, metric);
        }
    }
}