namespace MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;

public interface IAppMetricsLogger
{
    void TrackMetric(string key, int metric);
}