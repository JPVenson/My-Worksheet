using System;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Repository;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Home.Widgets;

public partial class ProjectTrackerWidget
{
    [Inject]
    public HttpService HttpService { get; set; }

    [Inject]
    public ICacheRepository<GetProjectModel> ProjectsRepository { get; set; }

    public Guid? InitialProjectId { get; set; }

    public GetProjectModel[] Projects { get; set; }
    public GetProjectModel SelectedProject { get; set; }
    public WorksheetModel SelectedWorksheet { get; set; }
    public bool LoadingWorksheet { get; set; }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        await ProjectsRepository.Cache.LoadAll();
        Projects = ProjectsRepository.Cache.Where(p => !p.Hidden).ToArray();
        var projId = WidgetInstance.GetArg("projectId");
        InitialProjectId = projId is not null ? Guid.Parse(projId) : null;
        if (InitialProjectId.HasValue)
            await SelectProject(InitialProjectId.Value);
    }

    private async Task OnProjectSelected(ChangeEventArgs e)
    {
        if (!Guid.TryParse(e.Value?.ToString(), out var id))
        {
            ClearSelection();
            WidgetInstance.SetArg("projectId", null);
            return;
        }
        await SelectProject(id);
    }

    private async Task SelectProject(Guid id)
    {
        SelectedProject = Projects.FirstOrDefault(p => p.ProjectId == id);
        SelectedWorksheet = null;

        if (SelectedProject == null) return;

        LoadingWorksheet = true;
        StateHasChanged();
        var worksheets = ServerErrorManager.EvalAndUnbox(
            await HttpService.WorksheetApiAccess.GetByProject(SelectedProject.ProjectId, showHidden: false));
        LoadingWorksheet = false;

        // Pick the worksheet currently active (within its start/end window and not submitted)
        SelectedWorksheet = worksheets?
            .Where(w => !w.Submitted
                        && w.StartTime <= DateTimeOffset.UtcNow
                        && w.EndTime >= DateTimeOffset.UtcNow)
            .OrderByDescending(w => w.StartTime)
            .FirstOrDefault();
        WidgetInstance.SetArg("projectId", SelectedProject.ProjectId.ToString());
    }

    public void ClearSelection()
    {
        SelectedProject = null;
        SelectedWorksheet = null;
        LoadingWorksheet = false;
    }
}
