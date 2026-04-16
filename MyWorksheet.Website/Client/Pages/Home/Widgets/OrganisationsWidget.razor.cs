using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Services.Organisation;
using MyWorksheet.Website.Client.Util;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Organisation;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Home.Widgets;

public partial class OrganisationsWidget
{
    [Inject]
    public OrganizationService OrganizationService { get; set; }

    public IReadOnlyList<OrganizationSelectionViewModel> Organizations { get; set; }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        await OrganizationService.Organizations.Cache.LoadAll();
        Organizations = OrganizationService.Organizations.Cache.ToList();
    }
}
