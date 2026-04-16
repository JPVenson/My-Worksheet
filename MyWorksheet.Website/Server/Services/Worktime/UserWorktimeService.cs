using System.Linq;
using System;
using MyWorksheet.Website.Server.Models;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Services.Worktime;

[ScopedService(typeof(IUserWorktimeService))]
public class UserWorktimeService : IUserWorktimeService
{
    public UserWorkload GetDefault()
    {
        return new UserWorkload()
        {
            WorkTimeMode = 1,
            DayWorkTimeFriday = -1,
            DayWorkTimeMonday = -1,
            DayWorkTimeSaturday = -1,
            DayWorkTimeSunday = -1,
            DayWorkTimeThursday = -1,
            DayWorkTimeTuesday = -1,
            DayWorkTimeWednesday = -1,
            MonthWorktime = -1,
            WeekWorktime = -1,
            IdAppUser = Guid.Empty,
            IdProject = null,
            IdOrganisation = null,
            UserWorkloadId = Guid.Empty,
        };
    }

    public UserWorkload[] GetWorkloadsForProject(MyworksheetContext db, Guid[] projectIds, Guid userId)
    {
        return db.UserWorkloads
            .Where(f => projectIds.Contains(f.IdProject.Value))
            .Where(f => f.IdAppUser == userId)
            .ToArray();
    }

    public UserWorkload GetWorkloadForProject(MyworksheetContext db, Guid projectId, Guid userId)
    {
        var workload = db.UserWorkloads
            .Where(f => f.IdProject == projectId)
            .Where(f => f.IdAppUser == userId)
            .FirstOrDefault();

        if (workload == null)
        {
            workload = db.UserWorkloads
                .Where(f => f.IdProject == null)
                .Where(f => f.IdAppUser == userId)
                .FirstOrDefault();
        }

        return workload;
    }
}