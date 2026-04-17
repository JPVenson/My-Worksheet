using System;
using System.Linq;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Shared.Services.Logger;
using MyWorksheet.Website.Server.Shared.Services.Logging;
using MyWorksheet.Website.Server.Shared.Services.Logging.Default;
using MyWorksheet.Website.Server.Shared.TaskScheduling;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Services.ScheduledTasks;

[ScheduleOnDemand]
public class CleanupUndisposedTempLoggerTask : BaseTask
{
    private readonly DelegateLogger _logger;

    public CleanupUndisposedTempLoggerTask(DelegateLogger logger,
        IDbContextFactory<MyworksheetContext> dbContextFactory) : base(dbContextFactory)
    {
        _logger = logger;
    }

    public override void DoWork(TaskContext context)
    {
        var tempLoggers = _logger.OfType<TempLogger>().ToArray();
        foreach (var tempLogger in tempLoggers)
        {
            if (tempLogger.LastPurge < DateTime.UtcNow.AddMinutes(-5))
            {
                _logger.Remove(tempLogger);
            }
        }
        context.Logger.LogInformation(string.Format("Cleared {0} Loggers", tempLoggers.Length), LoggerCategories.ServerTask.ToString());
    }

    public override string NamedTask
    {
        get { return "Cleanup Undisposed Logger"; }
        protected set { }
    }
}