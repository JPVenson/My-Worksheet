using System.Collections.Generic;
using MyWorksheet.Website.Server.Shared.Services.Logger;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MyWorksheet.Webpage.Controller.Api;

public static class AiExceptionLogger
{
    public static IApplicationBuilder UseAiExceptionHandler(this IApplicationBuilder app, IHostEnvironment hostEnvironment)
    {
        return app.UseExceptionHandler(appError =>
        {
            var provider = appError.ApplicationServices.GetRequiredService<IAppInsightsProviderService>();
            var loggerFactory = appError.ApplicationServices.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("AiExceptionLogger");

            appError.Run(async context =>
            {
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                if (contextFeature != null)
                {
                    provider.TelemetryClient?.TrackException(contextFeature.Error);
                    logger.LogCritical("Unhandled Exception detected", LoggerCategories.Server.ToString(), new Dictionary<string, string>()
                    {
                        {
                            "Exception.ToString", contextFeature.Error.ToString()
                        },
                        {
                            "Exception.StackTrace", contextFeature.Error.StackTrace
                        }
                    });
                }

                if (contextFeature != null && hostEnvironment.IsDevelopment())
                {
                    var error = contextFeature.Error;
                    var serializeObject = JsonConvert.SerializeObject(error, Formatting.Indented);


                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(serializeObject);
                }
            });
        });
    }
}