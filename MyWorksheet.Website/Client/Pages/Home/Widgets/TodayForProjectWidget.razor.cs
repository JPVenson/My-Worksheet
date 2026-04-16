using System;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Repository;
using MyWorksheet.Website.Client.Services.UserWorkload;
using MyWorksheet.Website.Client.Services.WorksheetTracker;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.UserWorkload;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Home.Widgets;

public partial class TodayForProjectWidget
{
    [Inject]
    public HttpService HttpService { get; set; }

    [Inject]
    public ICacheRepository<GetProjectModel> ProjectsRepository { get; set; }

    [Inject]
    public UserWorkloadService UserWorkloadService { get; set; }

    [Inject]
    public WorksheetTrackerService WorksheetTrackerService { get; set; }

    public Guid? InitialProjectId { get; set; }

    public GetProjectModel[] Projects { get; set; }
    public GetProjectModel SelectedProject { get; set; }
    public GetUserWorkloadViewModel Workload { get; set; }
    public double TrackedMinutesToday { get; set; }
    public double ExpectedMinutesToday { get; set; }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        await ProjectsRepository.Cache.LoadAll();
        Projects = ProjectsRepository.Cache.Where(p => !p.Hidden).ToArray();
        await WorksheetTrackerService.Trackers.Load();
        WhenChanged(WorksheetTrackerService).Then(RefreshTrackedTime);
        AddDisposable(WorksheetTrackerService.OnFullMinute.Register(Render));

        var projId = WidgetInstance.GetArg("projectId");
        InitialProjectId = projId is not null ? Guid.Parse(projId) : null;
        if (InitialProjectId.HasValue)
            await SelectProject(InitialProjectId.Value);
    }

    private async Task OnProjectSelected(ChangeEventArgs e)
    {
        if (!Guid.TryParse(e.Value?.ToString(), out var id))
        {
            SelectedProject = null;
            Workload = null;
            WidgetInstance.SetArg("projectId", null);
            return;
        }
        await SelectProject(id);
    }

    private async Task SelectProject(Guid id)
    {
        SelectedProject = Projects.FirstOrDefault(p => p.ProjectId == id);
        Workload = null;

        if (SelectedProject != null)
        {
            Workload = await UserWorkloadService.GetWorkloadForProjectOrDefault(SelectedProject.ProjectId);
            ExpectedMinutesToday = TodayWorkloadWidget.GetExpectedMinutesForToday(Workload);
            RefreshTrackedTime();
            WidgetInstance.SetArg("projectId", SelectedProject.ProjectId.ToString());
        }
    }

    private void RefreshTrackedTime()
    {
        if (SelectedProject == null) return;
        TrackedMinutesToday = WorksheetTrackerService.Trackers
            .Where(t => t.IdProject == SelectedProject.ProjectId)
            .Sum(t => (DateTimeOffset.UtcNow - t.StartTime).TotalMinutes);
    }
}
