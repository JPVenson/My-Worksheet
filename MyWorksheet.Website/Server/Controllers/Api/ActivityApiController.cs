using System;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Webpage.Helper.Roles;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Activity;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Util.Auth;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Controllers.Api;

[Route("api/ActivityApi")]
[Authorize]
public class ActivityApiController : RestApiControllerBase<UserActivity, UserActivityViewModel>
{
    private readonly IActivityService _activityService;

    public ActivityApiController(IDbContextFactory<MyworksheetContext> dbContextFactory, IMapperService mapper, IActivityService activityService) : base(dbContextFactory, mapper)
    {
        _activityService = activityService;
    }

    public override IActionResult GetList()
    {
        return GetUserActivitys(false);
    }

    [Route("Admin/GetUserActivities")]
    [HttpGet]
    [Authorize(Roles = Roles.AdminRoleName)]
    public IActionResult GetUserActivitys(Guid userId, bool showHidden)
    {
        using var db = EntitiesFactory.CreateDbContext();
        var activitys = db.UserActivities
            .Where(f => f.IdAppUser == userId)
            .Where(f => f.Hidden == showHidden)
            .Where(e => e.DueDate == null || e.DueDate <= DateTime.UtcNow)
            .Where(f => f.Activated == true)
            .ToArray();

        return Data(MapperService.ViewModelMapper.Map<UserActivityViewModel[]>(activitys));
    }

    [Route("HideActivity")]
    [HttpPost]
    public async Task<IActionResult> HideActivity(Guid activityId)
    {
        using var db = EntitiesFactory.CreateDbContext();
        var activity = db.UserActivities.Find(activityId);
        if (activity == null || activity.IdAppUser != User.GetUserId())
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        await _activityService.HideActivity(activityId, User.GetUserId());

        //_db.Query().Update.Table<UserActivity>().Set.Column(f => f.Hidden).Value(false).Where(f => f.UserActivityId == activity).ExecuteNonQuery();
        return Data();
    }

    [Route("Admin/CreateUserActivitys")]
    [HttpPost]
    [Authorize(Roles = Roles.AdminRoleName)]
    public async Task<IActionResult> CreateUserActivitys([FromBody] UserActivityPostViewModel userActivity)
    {
        var model = MapperService.ViewModelMapper.Map<UserActivity>(userActivity);
        return Data(MapperService.ViewModelMapper.Map<UserActivityViewModel>(await _activityService.CreateActivity(model)));
    }

    [Route("CreateUserActivitys")]
    [HttpPost]
    public async Task<IActionResult> CreateUserActivity([FromBody] UserActivityViewModel userActivity)
    {
        var model = MapperService.ViewModelMapper.Map<UserActivity>(userActivity);
        model.IdAppUser = User.GetUserId();
        return Data(MapperService.ViewModelMapper.Map<UserActivityViewModel>(await _activityService.CreateActivity(model)));
    }

    [Route("GetUserActivities")]
    [HttpGet]
    public IActionResult GetUserActivitys(bool showHidden)
    {
        return GetUserActivitys(User.GetUserId(), showHidden);
    }
}