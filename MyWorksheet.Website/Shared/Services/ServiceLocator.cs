using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Services;
using Microsoft.Extensions.DependencyInjection;
using ServiceLocator.Discovery;

namespace MyWorksheet.Website.Shared.Services;

public static class ServiceLocatorHelper
{
    public static IServiceDiscoveryManager DiscoverInitServices(this IServiceDiscoveryManager serviceDiscoveryManager)
    {
        serviceDiscoveryManager.ServiceTypes.Add(new RequireInitDiscovery());
        return serviceDiscoveryManager;
    }

    private class RequireInitDiscovery : IServiceDiscovery
    {
        public IEnumerable<ServiceDescriptor> DiscoverServices(IServiceDiscoveryManager locator)
        {
            return locator.ServiceTypes.Where(e => e != this)
                .SelectMany(f => f.DiscoverServices(locator))
                .Where(e => e.ServiceType.IsClass)
                .Where(e => typeof(IRequireInit).IsAssignableFrom(e.ServiceType))
                .Select(serviceDiscovery => new ServiceDescriptor(typeof(IRequireInit), e => e.GetService(serviceDiscovery.ServiceType), ServiceLifetime.Singleton));
        }
    }

    public static async Task InitServices(IServiceProvider appApplicationServices,
                                          Func<int, int, string, ValueTask> progress = null)
    {
        var initServices = new List<object>();

        var toInitServices = appApplicationServices.GetServices<IRequireInit>()
                                                      .Distinct()
                                                      .OrderByDescending(e => e.Order)
                                                      .ToArray();

        for (var index = 0; index < toInitServices.Length; index++)
        {
            var requireInit = toInitServices[index];
            if (progress != null)
            {
                await progress.Invoke(index, toInitServices.Length, requireInit.GetType().ToString());
            }

            if (initServices.Contains(requireInit))
            {
                continue;
            }

            initServices.Add(requireInit);
            requireInit.Init();
            requireInit.Init(appApplicationServices);
            await requireInit.InitAsync();
            await requireInit.InitAsync(appApplicationServices);
        }
    }
}
