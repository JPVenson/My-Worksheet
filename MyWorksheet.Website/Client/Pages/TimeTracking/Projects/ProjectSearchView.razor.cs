using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.ChangeTracking;
using MyWorksheet.Website.Client.Services.ChargeRate;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Organisation;
using MyWorksheet.Website.Client.Services.Repository;
using MyWorksheet.Website.Client.Services.UserWorkload;
using MyWorksheet.Website.Client.Services.WaiterIndicator;
using MyWorksheet.Website.Client.Util;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Client.Util.View.List;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Organisation;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.UserWorkload;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.TimeTracking.Projects;

public partial class ProjectSearchView
{
    public ProjectSearchView()
    {
    }

    [Inject]
    public HttpService HttpService { get; set; }
    [Inject]
    public UserWorkloadService UserWorkloadService { get; set; }
    [Inject]
    public ChargeRateService ChargeRateService { get; set; }
    [Inject]
    public OrganizationService OrganizationService { get; set; }

    [Inject]
    public ICacheRepository<GetProjectModel> ProjectsRepository { get; set; }
    [Inject]
    public ICacheRepository<ProjectItemRateViewModel> ItemRateRepository { get; set; }

    public IList<ProjectViewModel> Projects { get; set; }

    public bool FilterHidden { get; set; }
    public string SearchText { get; set; }

    public IEnumerable<ProjectViewModel> FilteredProjects
    {
        get
        {
            var result = (Projects ?? Enumerable.Empty<ProjectViewModel>())
                .Where(p => p.ProjectSearchView.Hidden == FilterHidden);

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                result = result.Where(p =>
                    p.ProjectSearchView.Name?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true
                    || p.ProjectSearchView.NumberRangeEntry?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true
                    || p.ProjectSearchView.ProjectReference?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true
                    || p.Organization?.Name?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true);
            }

            return result;
        }
    }

    public override async Task LoadDataAsync()
    {
        await using (var tasks = new TaskList())
        {
            tasks.Add(ProjectsRepository.Cache.LoadAll());
            tasks.Add(ChargeRateService.ChargeRates.Load());
            tasks.Add(ItemRateRepository.Cache.LoadAll());
            tasks.Add(OrganizationService.Organizations.Cache.LoadAll());
            tasks.Add(UserWorkloadService.UserWorkloadRepository.Cache.LoadAll());
        }
        Projects = ProjectsRepository.Cache.Select(e => new ProjectViewModel(e)).ToArray();
        foreach (var projectViewModel in Projects)
        {
            projectViewModel.ChargeRates.AddRange(ItemRateRepository.Cache.Where(e => e.IdProject == projectViewModel.ProjectSearchView.ProjectId));
            projectViewModel.Workload = await UserWorkloadService.GetWorkloadForProjectOrDefault(projectViewModel.ProjectSearchView.ProjectId);
            if (projectViewModel.ProjectSearchView.IdOrganisation.HasValue)
            {
                projectViewModel.Organization =
                    await OrganizationService.Organizations.Cache.Find(projectViewModel.ProjectSearchView.IdOrganisation.Value);
            }
        }
    }

    public double WeekWorktime(ProjectViewModel projectViewModel)
    {
        if (projectViewModel.Workload == null)
        {
            return -1;
        }

        return UserWorkloadService.GetWeekWorktime(projectViewModel.Workload);
    }
}