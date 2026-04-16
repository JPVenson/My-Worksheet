using System.Threading.Tasks;
using System;
using MyWorksheet.Website.Server.Models;

namespace MyWorksheet.Website.Server.Services.Activity;

public interface IActivityService
{
    Task<UserActivity[]> CreateActivity(params UserActivity[] activity);
    Task DeleteActivity(Guid activityId, Guid idExecutingUser);
    Task HideActivity(Guid activityId, Guid idExecutingUser);
    Task<bool> ActivateActivity(UserActivity userActivity, MyworksheetContext db);
}