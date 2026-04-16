using Microsoft.ApplicationInsights;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Shared.Services.Logger;

public interface IAppInsightsProviderService
{
    TelemetryClient TelemetryClient { get; set; }
}


[SingletonService(typeof(IAppInsightsProviderService))]
public class AppInsightsProviderService : IAppInsightsProviderService
{
    public TelemetryClient TelemetryClient { get; set; }
}