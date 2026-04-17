using System;
using System.Diagnostics;
using System.Linq;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Shared.Services.Logger;
using MyWorksheet.Website.Server.Shared.Services.Logging;
using MyWorksheet.Website.Server.Shared.Services.Logging.Default;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace MyWorksheet.AppStartup;

public static class LoggerConfig
{
    public static void UseLogger(
        IApplicationBuilder app,
        DelegateLogger loggerDelegation)
    {
        var bufferLogger = loggerDelegation.FirstOrDefault() as BufferLogger;
        loggerDelegation.Clear();
        loggerDelegation.Add(new AppInsigtsLogger(app.ApplicationServices.GetService<IAppInsightsProviderService>()));
        loggerDelegation.Add(new CriticalLoggerEventLogger((entry) =>
        {
            using var db = app.ApplicationServices.GetRequiredService<IDbContextFactory<MyworksheetContext>>().CreateDbContext();
            db.Add(new AppLoggerLog()
            {
                Message = entry.Message,
                Category = entry.Category,
                Level = entry.Level,
                DateCreated = entry.DateCreated,
                DateInserted = DateTime.UtcNow,
                AdditionalData = JsonConvert.SerializeObject(entry.OptionalData ?? new object())
            });
            db.SaveChanges();
            //if (HttpContext.Current?.GetOwinContext()?.Authentication?.User?.Identity?.GetUserId<int>() == 1)
            //{
            //	return;
            //}

            //activityService.CreateActivity(
            //   ActivityTypes.AppError.CreateActivity(dbEntities, entry.OptionalData?.Select(e => e.Key + " = " + e.Value).Aggregate((e,f) => e + "<br/>" + f), $"{entry.Level}: {entry.Category} - {entry.Message}", 1));
        }, "Warning", "Error", "Critical"));
        loggerDelegation.Add(new FormattedOutputLogger(Console.Out)
        {
            Enabled = () => Debugger.IsAttached
        });
        loggerDelegation.Add(new FormattedOutputLogger(data => Debug.WriteLine(data, "My-Worksheet"))
        {
            Enabled = () => Debugger.IsAttached
        });

        LoggerEntry logg;
        while (bufferLogger != null && bufferLogger.LoggerEntries.TryDequeue(out logg))
        {
            loggerDelegation.Log(logg.Message, logg.Category, logg.Level, logg.OptionalData, logg.DateCreated);
        }
    }
}