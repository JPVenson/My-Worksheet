using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Util;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Module;
using MyWorksheet.Website.Client.Services.Repository;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Client.Util.View.List;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Organisation;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.UserManagement;

[Route("/User/new")]
[Route("/User/{id:guid}")]
[Page("Links/Server.Users", IsNavbarView = false, PageIconCss = "fas fa-user-lock", Group = "Links/Server")]
[Authorize(Roles = "Administrator")]
public partial class UserManagementCreateView : NavigationPageBase
{
    [Inject]
    public HttpService HttpService { get; set; }

    [Parameter]
    public Guid? Id { get; set; }

    [Inject]
    public ICacheRepository<FeatureRegionViewModel> Regions { get; set; }

    public EntityState<AccountApiUserCreate> User { get; set; }
    public EntityState<AccountApiAdminGet> EditUser { get; set; }

    public PagedList<OrganizationSelectionViewModel> Organizations { get; set; }
    public UserQuotaViewModel[] UserQuotas { get; set; }
    public UserActionAdminGet[] UserActions { get; set; }

    public int ActionsPage { get; set; } = 1;
    public int ActionsPageSize { get; } = 10;
    public int ActionsTotalPages => (int)Math.Ceiling((UserActions?.Length ?? 0) / (double)ActionsPageSize);
    public IEnumerable<UserActionAdminGet> PagedActions =>
        UserActions?.Skip((ActionsPage - 1) * ActionsPageSize).Take(ActionsPageSize)
        ?? Array.Empty<UserActionAdminGet>();

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        TrackBreadcrumb(BreadcrumbService.AddModuleLink("Links/Server.Users"));

        if (Id.HasValue)
        {
            Organizations = new PagedList<OrganizationSelectionViewModel>(
                list => ServerErrorManager.Eval(HttpService.OrganizationApiAccess.AdminGetForUser(Id.Value, list.Page, list.PageSize, includeInactives: true).AsTask()),
                WaiterService)
            {
                PageSize = 15
            };
            TrackWhen().Changed(Organizations).ThenRefresh(this);

            var editUserTask = HttpService.AccountApiAccess.AdminApi.Get(Id.Value).AsTask();
            var quotasTask = HttpService.AccountApiAccess.AdminApi.GetCounterInfos(Id.Value).AsTask();
            var actionsTask = HttpService.AccountApiAccess.AdminApi.GetUserActions(Id.Value).AsTask();
            await using (var tasks = new TaskList())
            {
                tasks.Add(editUserTask);
                tasks.Add(quotasTask);
                tasks.Add(actionsTask);
                tasks.Add(Organizations.SearchAsync());
            }
            EditUser = ServerErrorManager.Eval(await editUserTask);
            UserQuotas = ServerErrorManager.EvalAndUnbox(await quotasTask);
            UserActions = ServerErrorManager.EvalAndUnbox(await actionsTask);
        }
        else
        {
            await Regions.Cache.LoadAll();
            User = new EntityState<AccountApiUserCreate>(new AccountApiUserCreate()
            {
                NeedPasswordReset = false,
                RegionId = Guid.Empty,
                Username = "Unset",
                UserPlainTextPassword = ""
            }, EntityListState.Added);
        }
    }

    public async Task Save()
    {
        using (WaiterService.WhenDisposed())
        {
            if (Id.HasValue)
            {
                var apiResult = ServerErrorManager.Eval(await HttpService.AccountApiAccess.AdminApi.UpdateAccount(EditUser.Entity));
                ServerErrorManager.DisplayStatus();
                if (apiResult.Success)
                {
                    EditUser = apiResult;
                }
            }
            else
            {
                var apiResult = ServerErrorManager.Eval(await HttpService.AccountApiAccess.AdminApi.CreateUser(User.Entity));
                ServerErrorManager.DisplayStatus();
                if (apiResult.Success)
                {
                    ModuleService.NavigateTo("/User/" + apiResult.Object.UserID);
                    return;
                }
            }
        }
    }
}
