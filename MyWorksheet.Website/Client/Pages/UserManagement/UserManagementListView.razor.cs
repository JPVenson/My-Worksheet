using System;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Module;
using MyWorksheet.Website.Client.Services.Repository;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.UserManagement;

[Route("/Users")]
[Page("Links/Server.Users", IsNavbarView = true, PageIconCss = "fas fa-user-lock", Group = "Links/Server")]
[Authorize(Roles = "Administrator")]
public partial class UserManagementListView : NavigationPageBase
{
    [Inject]
    public HttpService HttpService { get; set; }

    [Inject]
    public ICacheRepository<AccountApiAdminGet> Users { get; set; }

    public override async Task LoadDataAsync()
    {
        await Users.Cache.LoadAll();
    }
}
