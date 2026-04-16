using System;
using System.Collections.Generic;
using System.Linq;
using MyWorksheet.Webpage.Helper;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Blob;
using MyWorksheet.Website.Server.Shared.Services.Logger;
using MyWorksheet.Website.Server.Shared.TaskScheduling;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Services.ScheduledTasks;

[ScheduleOnDemand]
public class CleanupTestUsersTask : BaseTask
{
    private readonly IBlobManagerService _blobManagerService;

    public CleanupTestUsersTask(IDbContextFactory<MyworksheetContext> dbContextFactory, IBlobManagerService blobManagerService) : base(dbContextFactory)
    {
        _blobManagerService = blobManagerService;
        NamedTask = "Clean up Testusers";
    }

    public override void DoWork(TaskContext context)
    {
        using var db = DbContectFactory.CreateDbContext();

        var tempUsers = db.AppUsers
            .Where(f => f.IsTestUser && f.CreateDate < DateTime.UtcNow.AddDays(-1))
            .ToArray();
        var successfulCleared = 0;
        foreach (var tempUser in tempUsers)
        {
            context.Logger.LogInformation($"Cleanup user {tempUser.ContactName}", LoggerCategories.ServerTask.ToString());
            try
            {
                using var transaction = db.Database.BeginTransaction();
                AccountHelper.DeleteUser(tempUser, db, _blobManagerService);
                db.SaveChanges();
                transaction.Commit();
                successfulCleared++;
                context.Logger.LogInformation($"user {tempUser.ContactName} removed", LoggerCategories.ServerTask.ToString());
            }
            catch (Exception e)
            {
                context.Logger.LogError(string.Format("Failed to execute delete of the Test user {0}", tempUser.AppUserId), LoggerCategories.ServerTask.ToString(), new Dictionary<string, string>()
                {
                    {
                        "Exception", e.ToString()
                    }
                });
            }
        }
        context.Logger.LogInformation(string.Format("Cleared {1}/{0} Test users", tempUsers.Length, successfulCleared), LoggerCategories.ServerTask.ToString());
    }

    public override string NamedTask { get; protected set; }
}