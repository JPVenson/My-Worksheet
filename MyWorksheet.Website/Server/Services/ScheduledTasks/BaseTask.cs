using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Activity;
using MyWorksheet.Website.Server.Shared.Services.Logger;
using MyWorksheet.Website.Server.Shared.TaskScheduling;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MyWorksheet.Website.Server.Services.ScheduledTasks;

public abstract class BaseTask : ITask
{
    protected IDbContextFactory<MyworksheetContext> DbContectFactory { get; }

    protected BaseTask(IDbContextFactory<MyworksheetContext> dbContectFactory)
    {
        DbContectFactory = dbContectFactory;
    }

    public async Task Run(TaskContext context)
    {
        Exception failed = null;
        var sp = new Stopwatch();
        sp.Start();
        context.Logger.LogInformation(string.Format("Started {1} at {0}", DateTime.UtcNow, NamedTask), LoggerCategories.ServerTask.ToString());
        try
        {
            IsBusy = true;
            DoWork(context);
            await DoWorkAsync(context);
        }
        catch (Exception e)
        {
            failed = e;
            await ActivityTypes.ScheduledTaskFailedActivity.Create(DbContectFactory.CreateDbContext(), this, e.Message);
            throw;
        }
        finally
        {
            sp.Stop();
            var optionalData = new Dictionary<string, string>()
            {
                {"Duration", sp.Elapsed.ToString("G") },
            };
            if (failed != null)
            {
                optionalData["Exception"] = failed.ToString();
            }
            using (context.Logger.BeginScope(optionalData))
            {
                context.Logger.Log(failed == null ? LogLevel.Information : LogLevel.Error,
                    "[{Category}] {Message}",
                    LoggerCategories.ServerTask.ToString(),
                    string.Format("Stopped Task: " + (failed != null ? "Failed" : "") + " {1} at {0}", DateTime.UtcNow, NamedTask));
            }


            IsBusy = false;
        }
    }

    public virtual void DoWork(TaskContext context)
    {

    }
    public virtual Task DoWorkAsync(TaskContext context)
    {
        return Task.CompletedTask;
    }

    public bool IsBusy { get; protected set; }
    public abstract string NamedTask { get; protected set; }
}