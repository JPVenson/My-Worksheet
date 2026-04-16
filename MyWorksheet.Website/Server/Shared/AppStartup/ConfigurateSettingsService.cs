using System;
using System.Collections.Generic;
using System.Linq;
using MyWorksheet.Website.Server.Services;
using MyWorksheet.Website.Server.Services.ServerSettings;
using MyWorksheet.Website.Server.Shared.Services.Logger;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using MyWorksheet.Website.Shared.Services.Activation;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.UserSettings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Shared.AppStartup;

[SingletonService()]
public class ConfigurateSettingsService : RequireInit
{
    public ConfigurateSettingsService()
    {
        Order = 5000;
    }

    public override void Init(IServiceProvider services)
    {
        var activator = services.GetRequiredService<ActivatorService>();
        Action<ServerSettingsService, IAppLogger, IConfiguration> configurateSettings = ConfigurateSettings;
        activator.ActivateMethod(configurateSettings.Method, this);
    }

    public void ConfigurateSettings(ServerSettingsService settings,
        IAppLogger logger, IConfiguration configuration)
    {
        settings.Delimiter.Add("->");
        settings.FromConfiguration(configuration, "AppSettings").ToArray();
        settings.FromConnectionSettings(configuration).ToArray();
        settings.FromEnvironment().ToArray();

        var serverSettings = new ServerSettingsModel("Server");

        serverSettings.AddSetting("Server-Version", () => typeof(ConfigurateSettingsService).Assembly.GetName().Version);
        serverSettings.AddSetting("Model-Version", () => typeof(WorksheetUiOptions).Assembly.GetName().Version);
        var fsSetting = new ServerSettingsModel("FileSystem");
        fsSetting.AddSetting("CacheState", DateTime.UtcNow);
        serverSettings.CreateChild(fsSetting);
        serverSettings.CreateChild(new ServerSettingsModel("ContentManage")
            .AddSetting("CacheState", DateTime.UtcNow).AsReadOnly());
        serverSettings.CreateChild(new ServerSettingsModel("Trace").AddSetting("Database", false));
        settings.Root.AddChild(serverSettings);

        foreach (var keyValuePair in settings.Root.CompileFlat())
        {
            logger.LogInformation("Setting Init", LoggerCategories.Settings.ToString(),
                new Dictionary<string, string>
                {
                    {
                        "name", keyValuePair.Key
                    },
                    {
                        "value", keyValuePair.Value?.ToString() ?? "NULL"
                    }
                });
        }
    }

    //public static DbEntities CreateDbContext()
    //{
    //	DbEntities.AsyncDefault = false;
    //	DbEntities.ConfigureAwait = false;
    //	var entitys =
    //		new DbEntities(ServerSettingsService.Value.GetSetting<string>("ConnectionStrings.DefaultConnection"));
    //	entitys.LoadCompleteResultBeforeMapping = true;
    //	entitys.Async = false;
    //	entitys.ThreadSave = false;
    //	entitys.Multipath = true;
    //	if (!Debugger.IsAttached && !ServerSettingsService.Value.GetSetting<bool>("Server.Trace.Database"))
    //	{
    //		return entitys;
    //	}

    //	entitys.RaiseEvents = true;
    //	entitys.RaiseEventsAsync = true;

    //	entitys.OnDelete += Entitys_OnQuery;
    //	entitys.OnInsert += Entitys_OnQuery;
    //	entitys.OnNonResultQuery += Entitys_OnQuery;
    //	entitys.OnSelect += Entitys_OnQuery;
    //	entitys.OnUpdate += Entitys_OnQuery;

    //	return entitys;
    //}

    //private static void Entitys_OnQuery(object sender, DataAccess.DbEventArgs.DatabaseActionEvent e)
    //{
    //	var queryDebuggerStackTracer = e.QueryDebugger.StackTracer;
    //	IoC.Resolve<IAppLogger>()
    //		.LogVerbose("Query", LoggerCategories.Database.ToString(),
    //			new Dictionary<string, string>()
    //			{
    //				{"Query", e.QueryDebugger.DebuggerQuery}
    //			});
    //}
}