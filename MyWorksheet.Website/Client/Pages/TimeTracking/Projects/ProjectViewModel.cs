using System.Collections.Generic;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Organisation;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.UserWorkload;

namespace MyWorksheet.Website.Client.Pages.TimeTracking.Projects;

public class ProjectViewModel
{
    public ProjectViewModel(GetProjectModel projectSearchView)
    {
        ProjectSearchView = projectSearchView;
        ChargeRates = new List<ProjectItemRateViewModel>();
    }

    public GetProjectModel ProjectSearchView { get; set; }
    public OrganizationViewModel Organization { get; set; }
    public GetUserWorkloadViewModel Workload { get; set; }
    public List<ProjectItemRateViewModel> ChargeRates { get; set; }

    public ProjectItemRateViewModel DefaultRate()
    {
        return ChargeRates.Find(e => e.ProjectItemRateId == ProjectSearchView.IdDefaultRate);
    }
}