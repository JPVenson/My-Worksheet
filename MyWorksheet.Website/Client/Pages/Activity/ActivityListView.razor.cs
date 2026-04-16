using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Activity;
using MyWorksheet.Website.Client.Services.Breadcrumb;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Activity;

public partial class ActivityListView
{
    [Inject]
    public ActivityService ActivityService { get; set; }

    [Inject]
    public HttpService HttpService { get; set; }

    public UserActivityViewModel CurrentUserActivity { get; set; }
    public int ActivityIndex { get; set; }

    [Parameter]
    public string FromRoute { get; set; }

    public override async Task LoadDataAsync()
    {
        TrackBreadcrumb(BreadcrumbService.Add(new BreadcrumbPart()
        {
            Url = FromRoute,
            Path = FromRoute,
            Title = "Common/Back"
        }));

        await base.LoadDataAsync();

        WhenChanged(ActivityService.ActivitiesCache.Cache)
            .Then(() =>
            {
                ActivityIndex = ActivityService.ActivitiesCache.Cache.IndexOf(CurrentUserActivity);
            });
        await ActivityService.ActivitiesCache.Cache.LoadAll();
        CurrentUserActivity = ActivityService.ActivitiesCache.Cache.FirstOrDefault();

        ActivityIndex = ActivityService.ActivitiesCache.Cache.IndexOf(CurrentUserActivity);
    }

    public bool CanGoNext()
    {
        return ActivityIndex + 1 < ActivityService.ActivitiesCache.Cache.Count;
    }

    public bool CanGoPrevious()
    {
        return ActivityIndex - 1 >= 0;
    }

    public void GoNext()
    {
        CurrentUserActivity = ActivityService.ActivitiesCache.Cache.ElementAtOrDefault(++ActivityIndex);
    }

    public void GoPrevious()
    {
        CurrentUserActivity = ActivityService.ActivitiesCache.Cache.ElementAtOrDefault(--ActivityIndex);
    }

    private async Task CloseActivity()
    {
        using (WaiterService.WhenDisposed())
        {
            var activityIndex = ActivityIndex;
            var activityId = CurrentUserActivity.UserActivityId;
            var hideActivity = await HttpService.ActivityApiAccess.HideActivity(activityId);
            if (hideActivity.Success)
            {
                if (activityIndex != ActivityService.ActivitiesCache.Cache.Count)
                {
                    CurrentUserActivity = ActivityService.ActivitiesCache.Cache.ElementAtOrDefault(activityIndex);
                    ActivityIndex = activityIndex;
                }
                else if (activityIndex > 0)
                {
                    CurrentUserActivity = ActivityService.ActivitiesCache.Cache.ElementAtOrDefault(--activityIndex);
                    ActivityIndex = activityIndex;
                }
                else
                {
                    CurrentUserActivity = null;
                }
            }
        }
    }
}