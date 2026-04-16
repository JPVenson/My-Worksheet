using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Services.ChargeRate;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Organisation;
using MyWorksheet.Website.Client.Services.Workflow;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.TimeTracking.Projects;

public partial class ProjectWorksheetsComponent
{
    public ProjectWorksheetsComponent()
    {
        AddNewWorksheet = new WorksheetModel();
        CurrentMonthToEditor();
    }

    public void CurrentMonthToEditor()
    {
        var today = DateTimeOffset.UtcNow.Date;
        AddNewWorksheet.EndTime = today.AddDays(DateTime.DaysInMonth(today.Year, today.Month) - today.Day);
        AddNewWorksheet.StartTime = today.AddDays(today.Day * -1 + 1);
    }

    public void AddMonthToEditor()
    {
        var today = AddNewWorksheet.StartTime.AddMonths(1);
        AddNewWorksheet.EndTime = today.AddDays(DateTime.DaysInMonth(today.Year, today.Month) - today.Day);
        AddNewWorksheet.StartTime = today.AddDays(today.Day * -1 + 1);
    }

    public void EndsInOneWeek()
    {
        AddNewWorksheet.EndTime = AddNewWorksheet.StartTime;
        while (AddNewWorksheet.EndTime.Value.DayOfWeek != DayOfWeek.Sunday)
        {
            AddNewWorksheet.EndTime = AddNewWorksheet.EndTime.Value.AddDays(1);
        }
    }

    public void StartsAtCurrentWeek()
    {
        AddNewWorksheet.StartTime = DateTimeOffset.UtcNow.Date;
        while (AddNewWorksheet.StartTime.DayOfWeek != DayOfWeek.Monday)
        {
            AddNewWorksheet.StartTime = AddNewWorksheet.StartTime.AddDays(-1);
        }
    }

    [Inject]
    public WorkflowService WorkflowService { get; set; }
    [Inject]
    public HttpService HttpService { get; set; }

    [Parameter]
    public EditProjectViewModel Project { get; set; }

    public bool DisplayAddNew { get; set; }

    public WorksheetModel AddNewWorksheet { get; set; }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        WhenChanged(Project).ThenRefresh(this);
        WhenChanged(WorkflowService).ThenRefresh(this);
        AddNewWorksheet.IdProject = Project.Project.ProjectId;
        LayoutController.Modifier(e => e.FullHeightContent = true);
    }


    private async Task CreateNewWorksheet()
    {
        using (WaiterService.WhenDisposed())
        {
            ServerErrorManager.Clear();
            var result = ServerErrorManager.Eval(await HttpService.WorksheetApiAccess.Create(AddNewWorksheet).AsTask());
            ServerErrorManager.DisplayStatus();
            if (result.Success)
            {
                Project.Worksheets.Add(result.Object);
            }
        }
    }

    private void ToggleOpenEndedWorksheet()
    {
        if (AddNewWorksheet.EndTime.HasValue)
        {
            AddNewWorksheet.EndTime = null;
        }
        else
        {
            var today = DateTimeOffset.Now;
            AddNewWorksheet.EndTime = today.AddDays(DateTime.DaysInMonth(today.Year, today.Month) - today.Day);
        }
    }
}