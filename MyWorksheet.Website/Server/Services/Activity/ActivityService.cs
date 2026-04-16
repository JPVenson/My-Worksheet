using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Webpage.Services.WebHooks;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Services.ObjectChanged;
using MyWorksheet.Website.Server.Services.ScheduledTasks;
using MyWorksheet.Website.Server.Shared.TaskScheduling;
using MyWorksheet.Website.Server.Shared.TaskScheduling.TaskRunners;
using MyWorksheet.Website.Shared.Services.Activation;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;
using MyWorksheet.Website.Shared.ViewModels.Notifications;
using Microsoft.EntityFrameworkCore;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Services.Activity;

[SingletonService(typeof(IActivityService))]
public class ActivityService : IActivityService
{
    private readonly IDbContextFactory<MyworksheetContext> _dbFactory;
    private readonly IWebHookService _webHookService;
    private readonly IMapperService _mapperService;
    private readonly ObjectChangedService _objectChangedService;

    public ActivityService(IDbContextFactory<MyworksheetContext> dbFactory,
        ISchedulerService schedulerService,
        ActivatorService activatorService,
        IWebHookService webHookService,
        IMapperService mapperService,
        ObjectChangedService objectChangedService)
    {
        _dbFactory = dbFactory;
        _webHookService = webHookService;
        _mapperService = mapperService;
        _objectChangedService = objectChangedService;
        if (true)
        {
            _dynamicTaskRunner = schedulerService?.TryAddTask(
                activatorService.ActivateType<DueActivityActivatorTask>(this), () =>
                {
                    using var db = _dbFactory.CreateDbContext();
                    var nextActivity = db.UserActivities
                        .Where(f => f.DueDate != null && f.DueDate < DateTime.UtcNow.AddMinutes(10))
                        .Where(f => f.Activated == false)
                        .OrderBy(e => e.DueDate)
                        .FirstOrDefault();

                    return nextActivity?.DueDate ?? DateTime.UtcNow.AddDays(1);
                });
        }
    }

    private readonly DynamicTaskRunner _dynamicTaskRunner;

    public async Task<bool> ActivateActivity(UserActivity userActivity, MyworksheetContext db)
    {
        var firstOrDefault = ActivityTypes.Yield().FirstOrDefault(e => e.TypeKey == userActivity.ActivityType);
        if (firstOrDefault != null && !firstOrDefault.ActivityActivated(userActivity))
        {
            return false;
        }
        db.UserActivities.Where(e => e.UserActivityId == userActivity.UserActivityId).ExecuteUpdate(f => f.SetProperty(e => e.Activated, true));


        _webHookService.PublishEvent(WebHookTypes.ActivityWebHook,
            WebHookTypes.ActivityWebHook.CreateHookObject(
                _mapperService.ViewModelMapper.Map<UserActivityViewModel>(userActivity), ActionTypes.Created),
            userActivity.IdAppUser, "::1");
        await _objectChangedService.SendObjectChanged(ChangeEventTypes.Added, userActivity, null, userActivity.IdAppUser);
        return true;
    }

    public async Task<UserActivity[]> CreateActivity(params UserActivity[] activity)
    {
        var nActivitys = new List<UserActivity>();
        using var db = _dbFactory.CreateDbContext();
        foreach (var userActivity in activity.Where(e => e != null))
        {
            db.Add(userActivity);

            //is userActivity instant active?
            if (!userActivity.DueDate.HasValue || userActivity.DueDate.Value <= DateTime.UtcNow)
            {
                if (!await ActivateActivity(userActivity, db))
                {
                    continue;
                }
            }
            nActivitys.Add(userActivity);
        }
        db.SaveChanges();
        _dynamicTaskRunner.Reinvalidate();
        return nActivitys.ToArray();
    }

    public async Task HideActivity(Guid activityId, Guid idExecutingUser)
    {
        using var db = _dbFactory.CreateDbContext();
        db.UserActivities.Where(f => f.IdAppUser == idExecutingUser).Where(f => f.UserActivityId == activityId).ExecuteUpdate(e => e.SetProperty(f => f.Hidden, true));

        _webHookService.PublishEvent(WebHookTypes.ActivityWebHook,
            WebHookTypes.ActivityWebHook.CreateHookObject(
                new UserActivityViewModel()
                {
                    UserActivityId = activityId,
                    IdCreator = idExecutingUser,
                }, ActionTypes.Created),
            idExecutingUser, "::1");

        await _objectChangedService.SendObjectChanged(ChangeEventTypes.Removed, typeof(UserActivity), activityId, null, idExecutingUser);
        //AccessElement<ActivityHubInfo>.Instance.SendActivityChanged(idExecutingUser, new UserActivity() { UserActivityId = activityId }, ExternalActivityAction.Hidden);
    }

    public async Task DeleteActivity(Guid activityId, Guid idExecutingUser)
    {
        using var db = _dbFactory.CreateDbContext();
        var activity = db.UserActivities.Where(f => f.IdAppUser == idExecutingUser).Where(f => f.UserActivityId == activityId).FirstOrDefault();
        if (activity == null)
        {
            return;
        }

        db.UserActivities.Remove(activity);
        db.SaveChanges();
        await _objectChangedService.SendObjectChanged(ChangeEventTypes.Removed, typeof(UserActivity), activityId, null, idExecutingUser);

        //AccessElement<ActivityHubInfo>.Instance.SendActivityChanged(idExecutingUser, new UserActivity() { UserActivityId = activityId }, ExternalActivityAction.Hidden);
    }
}