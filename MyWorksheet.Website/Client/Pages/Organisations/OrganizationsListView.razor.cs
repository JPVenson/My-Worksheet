using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Util;
using MyWorksheet.Website.Client.Services.Organisation;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Organisation;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Organisations;

public partial class OrganizationsListView
{
    [Inject]
    public HttpService HttpService { get; set; }

    public bool IncludeInactive { get; set; }

    [Inject]
    public OrganizationService OrganizationService { get; set; }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        await using (var tasks = new TaskList())
        {
            tasks.Add(OrganizationService.Organizations.Cache.LoadAll());
            tasks.Add(OrganizationService.Roles.Load());
        }
        OrganizationService.Roles.WhenLoaded(Render);
    }

    public bool CanOpenOrganization(OrganizationSelectionViewModel org)
    {
        return org.IdRelations != null && org.IdRelations.Contains(OrganizationService.AdministratorRole?.Id ?? Guid.Empty);
    }

    private void NavigateToOrg(OrganizationSelectionViewModel org)
    {
        if (CanOpenOrganization(org))
        {
            NavigationService.NavigateTo("/Organization/" + org.OrganisationId);
        }
    }
}