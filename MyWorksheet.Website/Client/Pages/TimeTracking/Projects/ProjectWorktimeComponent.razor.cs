using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.UserWorkload;
using MyWorksheet.Website.Client.Services.WaiterIndicator;
using MyWorksheet.Website.Client.Util.View;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.UserWorkload;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace MyWorksheet.Website.Client.Pages.TimeTracking.Projects;

public partial class ProjectWorktimeComponent : NavigationPageBase
{
    private IWorktimeMode _worktimeMode;

    public ProjectWorktimeComponent()
    {

    }

    [Parameter]
    public EditProjectViewModel Project { get; set; }

    [Inject]
    public UserWorkloadService UserWorkloadService { get; set; }

    [Inject]
    public HttpService HttpService { get; set; }

    [Inject]
    public IServiceProvider ServiceProvider { get; set; }

    public IWorktimeMode WorktimeMode
    {
        get { return _worktimeMode; }
        set
        {
            SetProperty(ref _worktimeMode, value);
        }
    }

    public EditContext WorktimeEditContext { get; set; }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        WhenChanged(Project, Project.ProjectWorkload)
            .Changed(UserWorkloadService.UserWorkloadRepository.Cache)
            .ThenRefresh(this);

        WorktimeEditContext ??= new EditContext(this);
        WorktimeEditContext.EnableDataAnnotationsValidation(ServiceProvider);
    }

    public async Task ResetToGlobal()
    {
        ServerErrorManager.ServerErrors.Clear();
        using (WaiterService.WhenDisposed())
        {
            var result = ServerErrorManager.Eval(await HttpService.UserWorkloadApiAccess.Delete(Project.ProjectWorkload.UserWorkloadId));
            if (result.Success)
            {
                UserWorkloadService.UserWorkloadRepository.Cache.RemoveId(Project.ProjectWorkload.UserWorkloadId);
                Project.ProjectWorkloadState = await UserWorkloadService.GetWorkloadForProjectOrDefault(Project.Project.ProjectId);
                WaiterService.DisplayOk();
            }
        }
    }

    public async void Save()
    {
        ServerErrorManager.ServerErrors.Clear();
        using (WaiterService.WhenDisposed())
        {
            ApiResult<GetUserWorkloadViewModel> result;
            if (Project.ProjectWorkload.IdProject.HasValue)
            {
                result = ServerErrorManager.Eval(
                    await HttpService.UserWorkloadApiAccess.Update(Project.ProjectWorkload));
            }
            else
            {
                Project.ProjectWorkload.IdProject = Project.Project.ProjectId;
                result = ServerErrorManager.Eval(
                    await HttpService.UserWorkloadApiAccess.Create(Project.ProjectWorkload));
            }

            if (result.Success)
            {
                Project.ProjectWorkloadState = result.Object;
                WaiterService.DisplayOk();
            }
        }
    }
}