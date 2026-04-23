using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceLocator.Attributes;
using MyWorksheet.Website.Shared.Services;

namespace MyWorksheet.Website.Server.Shared.Services.Logging;

[SingletonService()]
public class DatabaseLoggerInstaller : RequireInit
{
    public DatabaseLoggerInstaller()
    {
        Order = 1;
    }

    public override void Init(IServiceProvider services)
    {
        var requiredService = services.GetRequiredService<DelegateLogger>();
        var scopeFactory = services.GetRequiredService<IServiceScopeFactory>();
        var lifetime = services.GetRequiredService<IHostApplicationLifetime>();
        requiredService.Add(new DatabaseLogger(scopeFactory, lifetime.ApplicationStopping));
    }
}
