using System;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MyWorksheet.AppStartup;
using MyWorksheet.Webpage.Controller.Api;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Migration;
using MyWorksheet.Website.Server.Shared.Hubs.Hubs.Server;
using MyWorksheet.Website.Server.Shared.Services.Logging;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using MyWorksheet.Website.Shared.Services;
using MyWorksheet.Website.Shared.Services.Activation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MimeKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using ServiceLocator.Discovery.Option;
using ServiceLocator.Discovery.Service;

namespace MyWorksheet.Website.Server;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCustomBearerAuthentification(Configuration);
        services.AddDbContextFactory<MyworksheetContext>(options =>
        {
            options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddSingleton<MigrationService>();

        services.AddControllersWithViews()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
            });
        services.AddRazorPages();
        services.AddSignalR(e => { e.KeepAliveInterval = TimeSpan.FromSeconds(20); });
        services.AddResponseCaching();
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();

            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
            {
                MimeTypes.GetMimeType(".wasm"),
                MimeTypes.GetMimeType(".dll"),
            });
        });
        services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Fastest;
        });

        services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.SmallestSize;
        });
        services.UseServiceDiscovery()
            .FromTypes(typeof(Startup).Assembly.GetExportedTypes())
            .FromTypes(typeof(ActivatorService).Assembly.GetExportedTypes())
            .DiscoverInitServices()
            .DiscoverOptions(Configuration)
            .FromTypes(typeof(Startup).Assembly.GetExportedTypes())
            .FromTypes(typeof(ActivatorService).Assembly.GetExportedTypes())
            .LocateServices();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        var activator = app.ApplicationServices.GetRequiredService<ActivatorService>();

        var logger = app.ApplicationServices.GetService<IAppLogger>();
        var requiredService = app.ApplicationServices.GetRequiredService<ILogger<Startup>>();
        logger.Add(new AppDelegateLogger(requiredService));
        var migrationService = app.ApplicationServices.GetRequiredService<MigrationService>();
        migrationService.MigrateStepAsync(MigrationStageTypes.PreInitialisation, app.ApplicationServices).ConfigureAwait(true).GetAwaiter().GetResult();
        migrationService.MigrateStepAsync(MigrationStageTypes.CoreInitialisation, app.ApplicationServices).ConfigureAwait(true).GetAwaiter().GetResult();
        ServiceLocatorHelper.InitServices(app.ApplicationServices).GetAwaiter().GetResult();
        migrationService.MigrateStepAsync(MigrationStageTypes.AppInitialisation, app.ApplicationServices).ConfigureAwait(true).GetAwaiter().GetResult();


        if (env.IsDevelopment())
        {
            //app.UseDeveloperExceptionPage();
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseAiExceptionHandler(env);

        app.UseResponseCaching();
        app.UseResponseCompression();
        // app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseCustomBearerAuthentification();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapControllers();
            endpoints.MapStaticAssets();
            var app = endpoints.CreateApplicationBuilder();
            app.Use(next => context =>
            {
                if (context.Request.Path.HasValue && context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    return Task.CompletedTask;
                }

                context.Request.Path = "/index.html";
                // Set endpoint to null so the static files middleware will handle the request.
                context.SetEndpoint(null);

                return next(context);
            });
            app.UseStaticFiles();
            endpoints.MapFallback(app.Build());
            //endpoints.MapFallbackToFile("index.html");

            var method = typeof(HubEndpointRouteBuilderExtensions).GetMethods()
                .OrderBy(e => e.GetParameters().Length).FirstOrDefault(e => e.Name == "MapHub");

            foreach (var hubType in typeof(HubNameAttribute).Assembly.GetTypes()
                         .Where(e => typeof(Hub).IsAssignableFrom(e)))
            {
                var name = hubType.GetCustomAttribute<HubNameAttribute>();
                method.MakeGenericMethod(hubType).Invoke(null, new object[] { endpoints, "/hubs/" + name.Name });
            }
        });

        logger.Add(activator.ActivateType<SignalLogger>());
    }
}