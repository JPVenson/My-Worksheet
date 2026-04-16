using MyWorksheet.Website.Server.Models;

using System;
namespace MyWorksheet.Website.Server.Services.Worktime;

public interface IUserWorktimeService
{
    UserWorkload GetWorkloadForProject(MyworksheetContext db, Guid projectId, Guid userId);
    UserWorkload[] GetWorkloadsForProject(MyworksheetContext db, Guid[] projectIds, Guid userId);
}