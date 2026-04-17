using MyWorksheet.Website.Server.Shared.Services.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MyWorksheet.Website.Server;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(config =>
            {
                //config.AddJsonFile($"appsettings.Development.json", false);
            })
            .ConfigureServices(collection =>
            {
                collection.AddSingleton<DelegateLogger>();
            })
            .ConfigureLogging(e =>
            {
                e.AddConsole();
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}