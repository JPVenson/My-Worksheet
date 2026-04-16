using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using MyWorksheet.Website.Client.Services.Repository;
using MyWorksheet.Website.Server.Services;
using MyWorksheet.Website.Shared.Services;
using MyWorksheet.Website.Shared.Services.Activation;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using ServiceLocator.Discovery;
using ServiceLocator.Discovery.Service;

namespace MyWorksheet.Website.Client;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("RunApp");
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        builder.RootComponents.Add<App>("app");
        builder.RootComponents.Add<HeadOutlet>("head::after");
        builder.Services.AddSingleton(sp =>
        {
            var baseAddress = builder.HostEnvironment.BaseAddress;
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseAddress),
            };
            return httpClient;
        });

        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddAuthorizationCore();
        builder.Services.AddSingleton(typeof(ICacheRepository<>), typeof(LocalCacheRepository<>));
        builder.Services.UseServiceDiscovery()
            .FromAssembly(typeof(Program).Assembly)
            .FromAssembly(typeof(ActivatorService).Assembly)
            .DiscoverInitServices()
            .LocateServices();

        //enable to attach debugger on start
        //Debugger.Break();
        var webAssemblyHost = builder.Build();
        var jsRuntime = webAssemblyHost.Services.GetRequiredService<IJSRuntime>();
        await ServiceLocatorHelper.InitServices(webAssemblyHost.Services,
            (current, max, name) =>
            {
                Console.WriteLine("Init service: " + name);
                return LogMessageToUi(jsRuntime, $"Service {current}/{max}");
            });
        await webAssemblyHost.RunAsync();
    }

    public static ValueTask LogMessageToUi(IJSRuntime jsRuntime, string message)
    {
        return jsRuntime.InvokeVoidAsync("window.UpdateWaiterText", message);
    }
}