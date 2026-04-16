namespace MyWorksheet.Website.Server.Services.Migration;

using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

internal class CodeMigration(Type migrationType, MigrationAttribute metadata)
{
    public Type MigrationType { get; } = migrationType;

    public MigrationAttribute Metadata { get; } = metadata;

    public string BuildCodeMigrationId()
    {
        return Metadata.Order.ToString("yyyyMMddHHmmsss", CultureInfo.InvariantCulture) + "_" + Metadata.Name!;
    }

    private IServiceCollection MigrationServices(IServiceProvider serviceProvider, ILogger logger)
    {
        var childServiceCollection = new ServiceCollection()
            .AddSingleton(serviceProvider)
            .AddSingleton(logger);

        foreach (ServiceDescriptor service in serviceProvider.GetRequiredService<IServiceCollection>())
        {
            if (service.Lifetime == ServiceLifetime.Singleton && !service.ServiceType.IsGenericTypeDefinition)
            {
                object? serviceInstance = serviceProvider.GetService(service.ServiceType);
                if (serviceInstance != null)
                {
                    childServiceCollection.AddSingleton(service.ServiceType, serviceInstance);
                    continue;
                }
            }

            childServiceCollection.Add(service);
        }

        return childServiceCollection;
    }

    public async Task Perform(IServiceProvider? serviceProvider, ILogger logger, CancellationToken cancellationToken)
    {
        if (typeof(IAsyncMigrationRoutine).IsAssignableFrom(MigrationType))
        {
            if (serviceProvider is null)
            {
                await ((IAsyncMigrationRoutine)Activator.CreateInstance(MigrationType)!).PerformAsync(cancellationToken).ConfigureAwait(false);
            }
            else
            {
                using var migrationServices = MigrationServices(serviceProvider, logger).BuildServiceProvider();
                await ((IAsyncMigrationRoutine)ActivatorUtilities.CreateInstance(migrationServices, MigrationType)).PerformAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        else
        {
            throw new InvalidOperationException($"The type {MigrationType} does not implement either IMigrationRoutine or IAsyncMigrationRoutine and is not a valid migration type");
        }
    }
}
