using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Typeahead;
using MyWorksheet.Website.Client.Util;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Services.ChargeRate;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Organisation;
using MyWorksheet.Website.Client.Services.Repository;
using MyWorksheet.Website.Client.Services.UserWorkload;
using MyWorksheet.Website.Client.Services.WaiterIndicator;
using MyWorksheet.Website.Client.Services.Workflow;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Client.Util.View;
using MyWorksheet.Website.Shared.Services.Activation;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Organisation;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.UserWorkload;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace MyWorksheet.Website.Client.Pages.TimeTracking.Projects;

public partial class ProjectDetailView : NavigationPageBase
{
    public ProjectDetailView()
    {

    }

    [Parameter]
    public Guid? ProjectId { get; set; }

    [Inject]
    public OrganizationService OrganizationService { get; set; }
    [Inject]
    public UserWorkloadService UserWorkloadService { get; set; }

    [Inject]
    public ICacheRepository<GetProjectModel> ProjectsRepository { get; set; }
    [Inject]
    public ICacheRepository<GetUserWorkloadViewModel> UserWorkloadRepository { get; set; }

    [Inject]
    public ActivatorService ActivatorService { get; set; }

    public EditProjectViewModel Project { get; set; }

    public override async Task LoadDataAsync()
    {
        Project = null;
        await base.LoadDataAsync();
        TrackBreadcrumb(BreadcrumbService.AddModuleLink("Links/Timeboard.Projects"));
        if (!ProjectId.HasValue)
        {
            var state = new EntityState<GetProjectModel>(new GetProjectModel()
            {
                Name = "New Project",
                IdOrganisation = null,
                UserOrderNo = 0,
                ProjectId = Guid.Empty,
                IdDefaultRate = null,
                IdWorksheetWorkflow = null,
                IdWorksheetWorkflowDataMap = null,
                NoModifications = false,
                NumberRangeEntry = null
            }, EntityListState.Added);
            Project = ActivatorService.ActivateType<EditProjectViewModel>(state);
            Project.AddNewRate();
            await SetTitleAsync(new LocalizableString("Links/Timeboard.Project.Title", new LocalizableString("Common/New")));
        }
        else
        {
            var project = await ProjectsRepository.Cache.Find(ProjectId.Value);
            if (project == null)
            {
                return;
            }

            var proj = ActivatorService.ActivateType<EditProjectViewModel>(new EntityState<GetProjectModel>(project));

            AddDisposable(proj.Rates.WhenLoaded(Render));

            await SetTitleAsync(new LocalizableString("Links/Timeboard.Project.Title", proj.Project.Name));

            if (proj.Project.IdOrganisation.HasValue)
            {
                proj.Organization = await OrganizationService.Organizations.Cache.Find(proj.Project.IdOrganisation.Value);
            }

            var workloadTask = UserWorkloadService.GetWorkloadForProjectOrDefault(proj.Project.ProjectId).AsTask();
            await using (var tasks = new TaskList())
            {
                tasks.Add(proj.Worksheets.Load());
                tasks.Add(proj.Rates.Load());
                tasks.Add(workloadTask);
            }
            proj.ProjectWorkloadState = workloadTask.Result;
            Project = proj;
        }
    }
}